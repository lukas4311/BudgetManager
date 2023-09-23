import csv
from datetime import datetime
from dataclasses import dataclass
from typing import List

import pandas as pd
import pyodbc

import secret


@dataclass
class CoinbaseReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    total_unit: str
    operationType: str


class CoinbaseParser:

    def map_csv_row_to_model(self, row):
        time = row['created at']
        buy_or_sell = row['side']
        size = row['size']
        total = abs(float(row['total']))
        total_unit = row['price/fee/total unit']
        size_unit = row['size unit']

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_convert("utc")

        return CoinbaseReportData(pandas_date, size_unit, size, total, total_unit, buy_or_sell)

    def load_coinbase_report_csv(self):
        with open("..\\BrokerReports\\Coinbase.csv", 'r') as file:
            rows = csv.DictReader(file)
            records = []
            for row in rows:
                coinbase_record = self.map_csv_row_to_model(row)
                records.append(coinbase_record)

        return records


class CryptoSqlService:
    def ticker_exists(self, ticker: str):
        print(f'Loading if {ticker} exists')
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        stock_ticker_sql = """SELECT [Id] FROM [dbo].[StockTicker] WHERE [Ticker] = ?"""
        ticker_data = pd.read_sql_query(stock_ticker_sql, conn, params=[ticker])

    def create_new_ticker(self, ticker: str):
        print(f'Create new ticker {ticker}')

    def store_trade_data(self, crypto_trade_data: List[CoinbaseReportData]):
        print("Storing all trade data to DB")
        for trade in crypto_trade_data:
            print(trade)

parser = CoinbaseParser()
parsed_records = parser.load_coinbase_report_csv()
