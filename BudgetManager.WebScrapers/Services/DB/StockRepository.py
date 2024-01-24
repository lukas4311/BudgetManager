from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Orm.BrokerReportToProcess import BrokerReportToProcess
from Orm.BrokerReportToProcessState import BrokerReportToProcessState
from Orm.BrokerReportType import BrokerReportType
from Orm.CurrencySymbol import CurrencySymbol
from Orm.StockTicker import Base, StockTicker


connectionString = f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes'

class StockRepository:
    def get_ticker_id(self, ticker: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(StockTicker).where(StockTicker.ticker == ticker)
        ticker_model = session.scalars(stmt).first()
        return ticker_model.id if ticker_model is not None else None

    def create_new_ticker(self, ticker: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)

        insert_command = insert(StockTicker).values(ticker=ticker, name=ticker)
        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()

    def get_currency_id(self, currency_code: str):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(CurrencySymbol).where(CurrencySymbol.symbol == currency_code)
        currency_model = session.scalars(stmt).first()
        return currency_model.id if currency_model is not None else None

    def get_all_crypto_broker_reports_to_process(self):
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        broker_type_cmd = select(BrokerReportType).where(BrokerReportType.code == "Stock")
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