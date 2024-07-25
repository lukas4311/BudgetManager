import csv
import pandas as pd
import logging
from datetime import datetime
from dataclasses import dataclass
from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session
import secret
from Models.TradingReportData import TradingReportData
from Orm.CurrencySymbol import CurrencySymbol
from Orm.StockTicker import StockTicker
from Orm.StockTradeHistory import Base, StockTradeHistory
from ReportParsers.BrokerReportParser import BrokerReportParser
from Services.DB.StockRepository import StockRepository
from Services.YahooService import YahooService

log_name = 'Logs/IB.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


@dataclass
class IBReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    currency: str
    name: str


class InteractiveBrokersParse(BrokerReportParser):
    __stockRepo: StockRepository

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData:
        if self.__check_row(row):
            return None

        currency = row[None][0]
        ticker = row[None][1]
        date = row[None][2]
        date = date.split(',')[0]
        number_of_shares = float(row[None][3])
        total_without_fee = float(row[None][6])
        total = float(row[None][8])
        buy = total_without_fee < 0
        total_with_action = total * (-1 if buy else 1)

        pandas_date = pd.to_datetime(date)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")
        currency_id = self.__stockRepo.get_currency_id(currency)

        return TradingReportData(pandas_date, ticker, ticker, number_of_shares, total_with_action, currency_id)

    def map_report_rows_to_model(self, rows) -> list(TradingReportData):
        records = []

        for row in rows:
            stock_record = self.map_report_row_to_model(row)

            if stock_record is not None:
                records.append(stock_record)

        return records

    def __check_row(self, row):
        return row['Statement'] == 'Trades' and row['Field Value'] == 'Stocks' and row['Header'] == 'Data'

    def read_report_csv_file(self):
        yahoo_service = YahooService()
        cached_names = {}
        parsed_data = []

        with open("..\\BrokerReports\\IB_report.csv", 'r') as file:
            rows = csv.DictReader(file)
            ib_records = []

            filtered_rows = [row for row in rows if
                             row['Statement'] == 'Trades' and row['Field Value'] == 'Stocks' and row[
                                 'Header'] == 'Data']

            for row in filtered_rows:
                currency = row[None][0]
                ticker = row[None][1]
                date = row[None][2]
                date = date.split(',')[0]
                size = float(row[None][3])
                total_without_fee = float(row[None][6])
                total = float(row[None][8])
                buy = total_without_fee < 0
                total_with_action = total * (-1 if buy else 1)

                cachedName = cached_names[ticker] if ticker in cached_names else None

                if cachedName == None:
                    companyName = yahoo_service.get_company_name(ticker)
                    name = companyName if companyName != None else ticker
                    cached_names[ticker] = name

                pandas_date = pd.to_datetime(date)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert('utc')

                model = IBReportData(pandas_date, ticker, size, total_with_action, currency, name)
                ib_records.append(model)
                parsed_data.append(model)

        return parsed_data

    def save_report_data_to_db(self):
        data = self.read_report_csv_file()

        for tradeData in data:
            try:
                self.store_trades(tradeData)
            except Exception as e:
                logging.info(
                    'Error while saving for ticker: ' + tradeData.ticker + 'and for time' + tradeData.time.strftime(
                        '%Y-%m-%d'))
                logging.error(e)

    def store_trades(self, trading_data: IBReportData):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(StockTicker).where(StockTicker.ticker == trading_data.ticker)
        ticker = session.scalars(stmt).first()

        if ticker == None:
            insert_command = insert(StockTicker).values(ticker=trading_data.ticker, name=trading_data.name)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()

            ticker = session.scalars(stmt).first()

        stock_ticker_id = int(ticker.id)
        currency_command = select(CurrencySymbol).where(CurrencySymbol.symbol == trading_data.currency)
        currency_record = session.scalars(currency_command).first()
        currency_id = currency_record.id

        insert_trade_command = insert(StockTradeHistory).values(tradeTimeStamp=trading_data.time.strftime('%Y-%m-%d'),
                                                                stockTickerId=stock_ticker_id,
                                                                tradeSize=trading_data.size,
                                                                tradeValue=trading_data.total,
                                                                currencySymbolId=currency_id,
                                                                userIdentityId=1)
        with engine.connect() as conn:
            conn.execute(insert_trade_command)
            conn.commit()

        session.close()

# parser = InteractiveBrokersParse()
# parser.save_report_data_to_db()
