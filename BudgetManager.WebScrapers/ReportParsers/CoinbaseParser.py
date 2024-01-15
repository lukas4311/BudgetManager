import csv
import io
from datetime import datetime
from dataclasses import dataclass
from typing import List
import pandas as pd
import pyodbc

from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

from Orm.BrokerReportToProcessState import BrokerReportToProcessState
from Orm.BrokerReportType import BrokerReportType
from Orm.BrokerReportToProcess import Base, BrokerReportToProcess

import secret
import base64 as b64


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

        if buy_or_sell == "BUY":
            total = total * -1

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

    def load_csv_from_db(self):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        broker_type_cmd = select(BrokerReportType).where(BrokerReportType.code == "Crypto")
        broker_type = session.scalars(broker_type_cmd).first()
        broker_type_id = broker_type.id

        broker_state_command = select(BrokerReportToProcessState).where(BrokerReportToProcessState.code == "InProcess")
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        broker_report_data_command = select(BrokerReportToProcess).where(
            BrokerReportToProcess.brokerReportTypeId == broker_type_id
            and BrokerReportToProcess.brokerReportToProcessStateId == broker_state_id)

        broker_report_data = session.scalars(broker_report_data_command).all()

        session.close()
        all_reports_data = []

        for report_data in broker_report_data:
            try:
                parsed_csv = b64.b64decode(report_data.fileContentBase64).decode('utf-8')
                rows = csv.DictReader(io.StringIO(parsed_csv))
                records = []
                for row in rows:
                    coinbase_record = self.map_csv_row_to_model(row)
                    records.append(coinbase_record)

                print(records)
                all_reports_data.append({"user_id": report_data.userIdentityId, "report_id": report_data.id, "data": records})
            except Exception as e:
                print("Error while parsing CSV", e)
                # TODO: update db and change state of process

        return all_reports_data


class CryptoSqlService:
    def get_ticker_id(self, ticker: str):
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        stock_ticker_sql = """SELECT [Id] FROM [dbo].[CryptoTicker] WHERE [Ticker] = ?"""
        ticker_data = pd.read_sql_query(stock_ticker_sql, conn, params=[ticker])
        conn.close()
        return ticker_data["Id"].values[0] if ticker_data["Id"].values.size > 0 else None

    def create_new_ticker(self, ticker: str):
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        cursor = conn.cursor()
        params = (ticker, ticker)
        cursor.execute('''INSERT INTO [dbo].[CryptoTicker]([Ticker], [Name]) VALUES(?,?)''', params)
        conn.commit()
        conn.close()

    def get_currency_id(self, currency: str):
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        currency_sql = """SELECT Id from CurrencySymbol where SYMBOL = ?"""
        currency_data = pd.read_sql_query(currency_sql, conn, params=[currency])
        conn.close()

        return currency_data["Id"].values[0] if currency_data["Id"].values.size > 0 else None

    def insert_crypto_trade(self, tradingData: CoinbaseReportData):
        print("Insert record")
        ticker_id = self.get_ticker_id(tradingData.ticker)
        currency_id = self.get_currency_id(tradingData.total_unit)

        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')

        cursor = conn.cursor()
        params = (tradingData.time.strftime('%Y-%m-%d'), ticker_id, float(tradingData.size), float(tradingData.total),
                  currency_id)
        # cursor.execute('''
        #                 INSERT INTO [dbo].[StockTradeHistory]([TradeTimeStamp],[StockTickerId],[TradeSize],[TradeValue],[CurrencySymbolId],[UserIdentityId])
        #                 VALUES (?,?,?,?,?,1)
        #             ''', params)
        # conn.commit()
        print(tradingData)
        print(params)
        conn.close()

    def store_trade_data(self, crypto_trade_data: List[CoinbaseReportData]):
        for trade in crypto_trade_data:
            ticker_id = self.get_ticker_id(trade.ticker)

            if not ticker_id:
                self.create_new_ticker(trade.ticker)

            self.insert_crypto_trade(trade)

parser = CoinbaseParser()
report_data = parser.load_csv_from_db()

print(report_data)
# cryptoSqlService = CryptoSqlService()
#
# parsed_records = parser.load_coinbase_report_csv()
# cryptoSqlService.store_trade_data(parsed_records)
