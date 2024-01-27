from typing import List

import pandas as pd
import pyodbc
from sqlalchemy import create_engine, select, update, and_
from sqlalchemy.orm import Session

import secret
from Orm.BrokerReportToProcess import Base, BrokerReportToProcess
from Orm.BrokerReportToProcessState import BrokerReportToProcessState
from Orm.BrokerReportType import BrokerReportType
from Orm.CryptoTradeHistory import CryptoTradeHistory
from ReportParsers.CoinbaseParser import CoinbaseReportData


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
        currency_id = int(self.get_currency_id(tradingData.total_unit))

        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)
        crypto_trade = select(CryptoTradeHistory).where(and_(CryptoTradeHistory.tradeValue == tradingData.total
                                                             , CryptoTradeHistory.tradeSize == tradingData.size
                                                             , CryptoTradeHistory.cryptoTickerId == ticker_id
                                                             , CryptoTradeHistory.tradeTimeStamp == tradingData.time)
                                                        )
        crypto_data = session.scalars(crypto_trade)
        crypto_trade = crypto_data.first()

        if crypto_trade is None:
            conn = pyodbc.connect(
                f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')

            cursor = conn.cursor()
            params = (
                tradingData.time, ticker_id, float(tradingData.size), float(tradingData.total),
                currency_id)
            cursor.execute('''
                            INSERT INTO [dbo].[CryptoTradeHistory]([TradeTimeStamp],[CryptoTickerId],[TradeSize],[TradeValue],[CurrencySymbolId],[UserIdentityId])
                            VALUES (?,?,?,?,?,1)
                        ''', params)
            conn.commit()
            conn.close()
        else:
            print('Trade is already saved.')

    def store_trade_data(self, crypto_trade_data: List[CoinbaseReportData]):
        for trade in crypto_trade_data:
            ticker_id = self.get_ticker_id(trade.ticker)

            if not ticker_id:
                self.create_new_ticker(trade.ticker)

            self.insert_crypto_trade(trade)

    def changeProcessState(self, broker_report_id: int, state_code: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        broker_state_command = select(BrokerReportToProcessState).where(
            BrokerReportToProcessState.code == state_code)
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        update_command = update(BrokerReportToProcess).where(
            BrokerReportToProcess.id == broker_report_id).values(
            brokerReportToProcessStateId=broker_state_id)

        with engine.connect() as conn:
            conn.execute(update_command)
            conn.commit()

        session.close()