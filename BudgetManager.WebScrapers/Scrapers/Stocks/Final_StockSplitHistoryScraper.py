import pandas as pd
from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Orm.StockSplit import StockSplit
from Orm.StockTicker import Base, StockTicker
from Services.YahooService import YahooService, StockSplitData
from SourceFiles.stockList import stockToDownload
from datetime import timedelta
import time
import logging


class StockSplitScraper:
    def scrape_stocks_splits(self, ticker: str, split_data: list[StockSplitData]):
        try:
            split_data_to_save: list[StockSplitData] = []
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

        stmt = select(StockTicker).where(StockTicker.ticker == ticker)
        ticker_model = session.scalars(stmt).first()

        if ticker_model is None:
            print('Not found ticker')
            insert_command = insert(StockTicker).values(ticker=ticker, name=ticker)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()

            ticker_model = session.scalars(stmt).first()

        stmt = (select(StockSplit).where(StockSplit.stockTickerId == ticker_model.id)
                .order_by(StockSplit.splitTimeStamp.desc()))
        ticker_split_model = session.scalars(stmt).first()

        return ticker_split_model, ticker_model.id

    def save_split_to_db(self, split: StockSplitData, ticker_id, session, engine):
        insert_command = insert(StockSplit).values(stockTickerId=ticker_id, splitTimeStamp=split.date,
                                                   splitTextInfo=split.split, splitCoefficient=split.split_coefficient)
        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()


class StockSplitManager:
    def __init__(self):
        self.__yahoo_service = YahooService()
        self.__split_scraper = StockSplitScraper()

    def scrape_split_data(self, ticker: str):
        split_data = self.__yahoo_service.get_stock_split_history(ticker, '511056000', '1696896000')

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker, split_data)
            time.sleep(3)

    def scrape_split_data_to_all_predefined_ticker(self):
        tickers_to_scrape = stockToDownload
        for ticker in tickers_to_scrape:
            self.store_split_data(ticker)

    def store_split_data(self, ticker: str):
        split_data = self.__yahoo_service.get_stock_split_history(ticker, '511056000', '1696896000')

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker, split_data)
            time.sleep(3)
            if len(split_data) != 0:
                self.__split_scraper.scrape_stocks_splits(ticker, split_data)
                time.sleep(3)


# tickersToScrape = stockToDownload
# yahooService = YahooService()
#
# stockSplitScraper = StockSplitScraper()
#
# for ticker in tickersToScrape:
#     split_data = yahooService.get_stock_split_history(ticker, '511056000', '1696896000')
#
#     if len(split_data) != 0:
#         stockSplitScraper.scrape_stocks_splits(ticker, split_data)
#         time.sleep(3)