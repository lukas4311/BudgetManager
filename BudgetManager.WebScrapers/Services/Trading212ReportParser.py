import csv
from datetime import datetime
from dataclasses import dataclass
import pandas as pd
import logging
import pyodbc

import secret
from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton
from secret import influxDbUrl

log_name = 'Logs/app.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "StocksRoic", token, organizaiton, logging)


@dataclass
class TradingReportData:
    time: datetime
    ticker: str
    name: str
    number_of_shares: float
    total: float


class Trading212ReportParser:
    def read_report_csv_file(self):
        parsedData: list(TradingReportData) = [];
        with open("..\\BrokerReports\\Trading212_1.csv", 'r') as file:
            rows = csv.DictReader(file)
            for row in rows:
                time = row["Time"]
                ticker = row["Ticker"]
                name = row["Name"]
                number_of_shares = row["No. of shares"]
                total = row["Total (CZK)"]

                pandas_date = pd.to_datetime(time)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert("utc")

                try:
                    model = TradingReportData(pandas_date, ticker, name, number_of_shares, total)
                    parsedData.append(model)
                except Exception as e:
                    print(ticker + " - error - " + e)
        return parsedData

    def save_report_data_to_db(self):
        data = self.read_report_csv_file()

        for tradeData in data:
            try:
                self.store_orders(tradeData)
            except Exception as e:
                logging.info('Error while saving for ticker: ' + tradeData.ticker + 'and for time' + tradeData.time.strftime('%Y-%m-%d'))
                logging.error(e)


    def store_orders(self, tradingData: TradingReportData):
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        stock_ticker_sql = """SELECT [Id] FROM [dbo].[StockTicker] WHERE [Ticker] = ?"""
        ticker_data = pd.read_sql_query(stock_ticker_sql, conn, params=[tradingData.ticker])

        if len(ticker_data.index) == 0:
            cursor = conn.cursor()
            params = (tradingData.ticker, tradingData.name)
            cursor.execute('''
                                INSERT INTO [dbo].[StockTicker]([Ticker], [Name])
                                VALUES(?,?)
                            ''', params)
            conn.commit()

        ticker_data = pd.read_sql_query(stock_ticker_sql, conn, params=[tradingData.ticker])
        stock_ticker_id = int(ticker_data['Id'].values[0])

        currency_sql = """SELECT Id from CurrencySymbol where SYMBOL = 'CZK'"""
        currency_data = pd.read_sql(currency_sql, conn)
        currency_id: int = int(currency_data['Id'].values[0])

        cursor = conn.cursor()
        params = (tradingData.time.strftime('%Y-%m-%d'), stock_ticker_id, float(tradingData.number_of_shares), float(tradingData.total), currency_id)
        cursor.execute('''
                            INSERT INTO [dbo].[StockTradeHistory]([TradeTimeStamp],[StockTickerId],[TradeSize],[TradeValue],[CurrencySymbolId],[UserIdentityId])
                            VALUES (?,?,?,?,?,1)
                        ''', params)
        conn.commit()

parser = Trading212ReportParser()
parser.save_report_data_to_db()
