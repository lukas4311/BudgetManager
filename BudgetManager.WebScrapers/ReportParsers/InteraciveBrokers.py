import csv
import pandas as pd
import logging
from datetime import datetime
from dataclasses import dataclass

from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Orm.CurrencySymbol import CurrencySymbol
from Orm.StockTicker import StockTicker
from Orm.StockTradeHistory import Base, StockTradeHistory
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


class InteractiveBrokersParse:
    def read_report_csv_file(self):
        yahoo_service = YahooService()
        cachedNames = {}
        parsedData = []

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
                totalWithoutFee = float(row[None][6])
                total = float(row[None][8])
                buy = totalWithoutFee < 0
                totalWithAction = total * (-1 if buy else 1)

                cachedName = cachedNames[ticker] if ticker in cachedNames else None

                if cachedName == None:
                    companyName = yahoo_service.get_company_name(ticker)
                    name = companyName if companyName != None else ticker
                    cachedNames[ticker] = name

                pandas_date = pd.to_datetime(date)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert('utc')

                model = IBReportData(pandas_date, ticker, size, totalWithAction, currency, name)
                ib_records.append(model)
                parsedData.append(model)

        return parsedData

    def save_report_data_to_db(self):
        data = self.read_report_csv_file()

        for tradeData in data:
            try:
                # print(tradeData)
                self.store_orders(tradeData)
            except Exception as e:
                logging.info(
                    'Error while saving for ticker: ' + tradeData.ticker + 'and for time' + tradeData.time.strftime(
                        '%Y-%m-%d'))
                logging.error(e)

    def store_orders(self, tradingData: IBReportData):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(StockTicker).where(StockTicker.ticker == tradingData.ticker)
        ticker = session.scalars(stmt).first()

        if ticker == None:
            insertCommand = insert(StockTicker).values(ticker=tradingData.ticker, name=tradingData.name)
            with engine.connect() as conn:
                conn.execute(insertCommand)
                conn.commit()

            ticker = session.scalars(stmt).first()

        stock_ticker_id = int(ticker.id)
        currencyCommand = select(CurrencySymbol).where(CurrencySymbol.symbol == tradingData.currency)
        currency_record = session.scalars(currencyCommand).first()
        currency_id = currency_record.id

        insertTradeCommand = insert(StockTradeHistory).values(tradeTimeStamp=tradingData.time.strftime('%Y-%m-%d'),
                                                         stockTickerId=stock_ticker_id, tradeSize=tradingData.size,
                                                         tradeValue=tradingData.total, currencySymbolId=currency_id,
                                                         userIdentityId=1)
        with engine.connect() as conn:
            conn.execute(insertTradeCommand)
            conn.commit()

        # Close the session.
        session.close()

        # params = (tradingData.time.strftime('%Y-%m-%d'), stock_ticker_id, float(tradingData.number_of_shares),
        #           float(tradingData.total), currency_id)
        # cursor.execute('''
        #                     INSERT INTO [dbo].[StockTradeHistory]([TradeTimeStamp],[StockTickerId],[TradeSize],[TradeValue],[CurrencySymbolId],[UserIdentityId])
        #                     VALUES (?,?,?,?,?,1)
        #                 ''', params)
        # conn.commit()


parser = InteractiveBrokersParse()
parser.save_report_data_to_db()
