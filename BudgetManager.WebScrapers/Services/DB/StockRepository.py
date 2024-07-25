from sqlalchemy import create_engine, select, insert, and_, update
from sqlalchemy.orm import Session
from typing import List
import secret
from Models.TradingReportData import TradingReportData
from Orm.BrokerReportToProcess import BrokerReportToProcess
from Orm.BrokerReportToProcessState import BrokerReportToProcessState
from Orm.BrokerReportType import BrokerReportType
from Orm.EnumItem import EnumItem
from Orm.EnumItemType import EnumItemType
from Orm.StockTicker import Base
from Orm.Trade import Trade

connectionString = f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes'


class StockRepository:
    def get_enum_type(self, code: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItemType).where(EnumItemType.code == code)
        trade_ticker_type = session.scalars(stmt).first()
        return trade_ticker_type.id if trade_ticker_type is not None else None

    def get_enums_by_type_id(self, enum_item_type_id):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItem).where(EnumItem.enumItemTypeId == enum_item_type_id)
        enums = session.scalars(stmt).all()
        return enums if enums is not None else None

    def _get_ticker_id(self, ticker: str, ticker_type: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        trade_ticker_type = self.get_enum_type(ticker_type)

        stmt = select(EnumItem).where(and_(EnumItem.code == ticker, EnumItem.enumItemTypeId == trade_ticker_type))
        ticker_model = session.scalars(stmt).first()

        return ticker_model.id if ticker_model is not None else None

    def _create_new_ticker(self, ticker: str, ticker_type: str):
        self._create_new_ticker(ticker, ticker, ticker_type)

    def _create_new_ticker(self, ticker: str, name: str, ticker_type: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        enum_item_type_ticker = self.get_enum_type(ticker_type)
        insert_command = insert(EnumItem).values(code=ticker, name=name, enumItemTypeId=enum_item_type_ticker)

        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()

    def get_currency_id(self, currency_code: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItemType).where(EnumItemType.code == 'CurrencySymbols')
        currency_symbols_type = session.scalars(stmt).first()

        stmt = select(EnumItem).where(
            EnumItem.code == currency_code and EnumItem.enumItemTypeId == currency_symbols_type)
        currency_model = session.scalars(stmt).first()
        return currency_model.id if currency_model is not None else None

    def _get_all_stock_broker_reports_to_process(self):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        broker_type_cmd = select(BrokerReportType).where(BrokerReportType.code == "Stock")
        broker_type = session.scalars(broker_type_cmd).first()
        broker_type_id = broker_type.id

        broker_state_command = select(BrokerReportToProcessState).where(BrokerReportToProcessState.code == "InProcess")
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        broker_report_data_command = select(BrokerReportToProcess).where(and_(
            BrokerReportToProcess.brokerReportTypeId == broker_type_id, BrokerReportToProcess.brokerReportToProcessStateId == broker_state_id))

        broker_report_data = session.scalars(broker_report_data_command).all()
        session.close()

        return broker_report_data

    def _insert_stock_trade(self, trading_data: TradingReportData, currency_id: int, user_id: int):
        ticker_id = int(self._get_ticker_id(trading_data.ticker, trading_data.trade_ticker_type_code))

        if ticker_id is None:
            print(f'Ticker does not exists {trading_data.ticker}')

        if currency_id is None:
            print('Throw exception')

        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)
        stock_trade = (select(Trade)
                       .where(and_(Trade.tradeValue == trading_data.total,
                                   Trade.tradeSize == trading_data.number_of_shares, Trade.tickerId == ticker_id,
                                   Trade.tradeTimeStamp == trading_data.time)
                              ))
        stock_data = session.scalars(stock_trade)
        stock_trade = stock_data.first()

        if stock_trade is None:
            insert_command = insert(Trade).values(tradeTimeStamp=trading_data.time, tickerId=ticker_id,
                                                  tradeSize=trading_data.number_of_shares,
                                                  tradeValue=trading_data.total, tradeCurrencySymbolId=currency_id,
                                                  userIdentityId=user_id)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()
        else:
            print('Trade is already saved.')

        session.close()

    def store_trade_data(self, trade_data: List[TradingReportData], user_id: int):
        for trade in trade_data:
            ticker_id = self._get_ticker_id(trade.ticker, trade.trade_ticker_type_code)

            if not ticker_id:
                self._create_new_ticker(trade.ticker, trade.name, trade.trade_ticker_type_code)

            self._insert_stock_trade(trade, trade.currency_id, user_id)

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