from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Orm.StockTicker import Base, StockTicker


class StockRepository:
    def get_ticker_id(self, ticker: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(StockTicker).where(StockTicker.ticker == ticker)
        ticker_model = session.scalars(stmt).first()
        return ticker_model.id if ticker is not None else None

    def create_new_ticker(self, ticker: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)

        insert_command = insert(StockTicker).values(ticker=ticker, name=ticker)
        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()

    def get_currency_id(self, currency_code: str):
        print(currency_code)
