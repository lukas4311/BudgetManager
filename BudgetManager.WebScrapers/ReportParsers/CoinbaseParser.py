import csv
import io
from datetime import datetime
from dataclasses import dataclass
from typing import List
import pandas as pd
import pyodbc

from sqlalchemy import create_engine, select, insert, update, and_
from sqlalchemy.orm import Session

from Orm.CryptoTradeHistory import CryptoTradeHistory
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


class ParseCsvError(Exception):
    pass


class CoinbaseParser:

    def map_csv_row_to_model(self, row):
        time = row['created at']
        buy_or_sell = row['side']
        size = float(row['size'])
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

    def get_all_crypto_broker_reports_to_process(self):
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

        return broker_report_data

    def insert_crypto_trade(self, tradingData: CoinbaseReportData):
        print("Insert record")

        ticker_id = int(self.get_ticker_id(tradingData.ticker))
        print(ticker_id)
        currency_id = self.get_currency_id(tradingData.total_unit)

        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)
        crypto_trade = select(CryptoTradeHistory).where(and_(CryptoTradeHistory.tradeValue == tradingData.total
                                                             , CryptoTradeHistory.tradeSize == tradingData.size
                                                             , CryptoTradeHistory.cryptoTickerId == ticker_id
                                                             , CryptoTradeHistory.tradeTimeStamp == tradingData.time))
        crypto_data = session.scalars(crypto_trade)
        print(crypto_data)
        crypto_trade = crypto_data.first()
        print(crypto_trade)

        if crypto_trade is None:
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
        else:
            print('Trade is already saved.')

    def store_trade_data(self, crypto_trade_data: List[CoinbaseReportData]):
        for trade in crypto_trade_data:
            ticker_id = self.get_ticker_id(trade.ticker)

            if not ticker_id:
                self.create_new_ticker(trade.ticker)

            self.insert_crypto_trade(trade)

    def changeProcessStateToParseError(self, broker_report_id: int, errorStateCode: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        broker_state_command = select(BrokerReportToProcessState).where(
            BrokerReportToProcessState.code == errorStateCode)
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        update_command = update(BrokerReportToProcess).where(
            BrokerReportToProcess.brokerReportToProcessStateId == broker_report_id).values(
            brokerReportToProcessStateId=broker_state_id)

        with engine.connect() as conn:
            conn.execute(update_command)
            conn.commit()


def process_report_data(cryptoSqlService: CryptoSqlService, parser: CoinbaseParser):
    broker_report_data = cryptoSqlService.get_all_crypto_broker_reports_to_process()
    all_reports_data = []

    for report_data in broker_report_data:
        try:
            parse_report_data_to_model(all_reports_data, parser, report_data)
        except Exception as e:
            cryptoSqlService.changeProcessStateToParseError(report_data.id, "ParsingError")

    print(all_reports_data)
    for parsed_report in all_reports_data:
        try:
            cryptoSqlService.store_trade_data(parsed_report["data"])
            # TODO: change status to processed
        except Exception as e:
            print(parsed_report)
            cryptoSqlService.changeProcessStateToParseError(parsed_report["report_id"], "SavinggError")


def parse_report_data_to_model(all_reports_data, parser, report_data):
    try:
        parsed_csv = b64.b64decode(report_data.fileContentBase64).decode('utf-8')
        rows = csv.DictReader(io.StringIO(parsed_csv))
        records = []
        for row in rows:
            coinbase_record = parser.map_csv_row_to_model(row)
            records.append(coinbase_record)

        all_reports_data.append({"user_id": report_data.userIdentityId, "report_id": report_data.id, "data": records})
    except Exception as e:
        raise ParseCsvError("Error while parsing CSV")


parser = CoinbaseParser()
cryptoSqlService = CryptoSqlService()
process_report_data(cryptoSqlService, parser)
