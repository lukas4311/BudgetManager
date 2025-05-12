from typing import List

import pandas as pd
from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Services.DB.Orm.EnumItem import EnumItem
from Services.DB.Orm.StockSplit import StockSplit
from Services.DB.Orm.StockTicker import Base, StockTicker
from Services.DB.StockRepository import StockRepository
from Services.YahooService import YahooService, StockSplitData
from SourceFiles.stockList import stock_to_download
from datetime import timedelta, datetime
import time
import logging


class StockSplitScraper:
    def __init__(self):
        self.__stock_repo = StockRepository()

    def scrape_stocks_splits(self, ticker: str, split_data: list[StockSplitData]):
        try:
            split_data_to_save: list[StockSplitData] = None
            last_split, ticker_id = self.get_last_ticker_stored_split(ticker)

            if last_split is None:
                print('Insert all splits')
                split_data_to_save = split_data
            else:
                print('Add only newer')
                pandas_last_split = pd.to_datetime(last_split.splitTimeStamp)
                pandas_last_split = pandas_last_split.tz_localize("Europe/Prague")
                pandas_last_split = pandas_last_split.tz_convert("utc")
                pandas_last_split = pandas_last_split + timedelta(days=1)
                split_data_to_save = [d for d in split_data if d.date > pandas_last_split]

            if len(split_data_to_save) != 0:
                engine = create_engine(
                    f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

                Base.metadata.create_all(engine)
                session = Session(engine)

                for split in split_data_to_save:
                    self.save_split_to_db(split, ticker_id, session, engine)
        except Exception as e:
            logging.info('Error while downloading price for ticker: ' + ticker)
            logging.error(e)

    def get_last_ticker_stored_split(self, ticker: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        ticker_id = self.__stock_repo.get_ticker_id(ticker, 'StockTradeTickers')

        if ticker is None:
            print('Not found ticker')
            insert_command = insert(StockTicker).values(ticker=ticker, name=ticker)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()

            ticker_id = self.__stock_repo.get_ticker_id(ticker, 'StockTradeTickers')

        stmt = (select(StockSplit).where(StockSplit.tickerId == ticker_id)
                .order_by(StockSplit.splitTimeStamp.desc()))
        ticker_split_model = session.scalars(stmt).first()

        return ticker_split_model, ticker_id

    def save_split_to_db(self, split: StockSplitData, ticker_id, session, engine):
        insert_command = insert(StockSplit).values(tickerId=ticker_id, splitTimeStamp=split.date,
                                                   splitTextInfo=split.split, splitCoefficient=split.split_coefficient)
        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()


class StockSplitManager:
    def __init__(self):
        self.__yahoo_service = YahooService()
        self.__split_scraper = StockSplitScraper()
        self.__stock_repo = StockRepository()

    def scrape_split_data(self, ticker: str):
        # FIXME: fix problem with max date in past time
        split_data = self.__yahoo_service.get_stock_split_history(ticker.code, '511056000', self.__convert_to_unix_timestamp(datetime.now()))

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker.code, split_data)
            time.sleep(3)

    def scrape_split_for_all_ticker_in_db(self):
        tickers_enum_type_id = self.__stock_repo.get_enum_type('StockTradeTickers')
        enums: List[EnumItem] = self.__stock_repo.get_enums_by_type_id(tickers_enum_type_id)
        for enum in enums:
            self.scrape_split_data(enum)

    def scrape_split_data_to_all_predefined_ticker(self):
        for ticker in stock_to_download:
            self.store_split_data(ticker)

    def store_split_data(self, ticker: str):
        #FIXME: fix problem with max date in past time
        split_data = self.__yahoo_service.get_stock_split_history(ticker, '511056000', self.__convert_to_unix_timestamp(datetime.now()))

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker, split_data)
            time.sleep(3)
            if len(split_data) != 0:
                self.__split_scraper.scrape_stocks_splits(ticker, split_data)
                time.sleep(3)

    def __convert_to_unix_timestamp(self, date: datetime):
        return int(time.mktime(date.timetuple()))
