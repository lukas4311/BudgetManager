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
    """
    A class responsible for scraping and storing stock split data from various sources.

    This class handles the retrieval, processing, and storage of stock split information
    for individual tickers, managing database interactions and data validation.
    """

    def __init__(self):
        """
        Initialize the StockSplitScraper with required dependencies.

        Sets up the stock repository for database operations.
        """
        self.__stock_repo = StockRepository()

    def scrape_stocks_splits(self, ticker: str, split_data: list[StockSplitData]):
        """
        Scrape and save stock split data for a specific ticker.

        This method processes stock split data by checking for existing records,
        filtering for new data, and saving only the splits that haven't been stored yet.

        Args:
            ticker (str): The stock ticker symbol (e.g., 'AAPL', 'GOOGL')
            split_data (list[StockSplitData]): List of stock split data objects containing
                                             split information to be processed

        Raises:
            Exception: Logs any errors that occur during the scraping process
        """
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
        """
        Retrieve the most recent stock split record for a given ticker.

        This method queries the database to find the latest split entry for the specified
        ticker. If the ticker doesn't exist, it creates a new ticker record.

        Args:
            ticker (str): The stock ticker symbol to search for

        Returns:
            tuple: A tuple containing:
                - StockSplit or None: The most recent split record for the ticker,
                  or None if no splits exist
                - int: The ticker ID from the database
        """
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
        """
        Save a single stock split record to the database.

        This method inserts a new stock split record with all relevant information
        including the split timestamp, text information, and coefficient.

        Args:
            split (StockSplitData): The stock split data object containing split details
            ticker_id (int): The database ID of the ticker
            session (Session): SQLAlchemy session for database operations
            engine: SQLAlchemy engine for database connection
        """
        insert_command = insert(StockSplit).values(tickerId=ticker_id, splitTimeStamp=split.date,
                                                   splitTextInfo=split.split, splitCoefficient=split.split_coefficient)
        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()


class StockSplitManager:
    """
    A high-level manager class for orchestrating stock split data collection and storage.

    This class coordinates between the Yahoo service for data retrieval, the scraper
    for data processing, and the repository for database operations.
    """

    def __init__(self):
        """
        Initialize the StockSplitManager with required services.

        Sets up the Yahoo service for data retrieval, split scraper for processing,
        and stock repository for database operations.
        """
        self.__yahoo_service = YahooService()
        self.__split_scraper = StockSplitScraper()
        self.__stock_repo = StockRepository()

    def scrape_split_data(self, ticker: str):
        """
        Scrape stock split data for a single ticker from Yahoo Finance.

        This method retrieves historical split data from a fixed start date
        (Unix timestamp 511056000) to the current date and processes it.

        Args:
            ticker (str): The stock ticker symbol to scrape data for

        Note:
            Includes a 3-second delay after processing to avoid rate limiting.
            Contains a FIXME comment indicating an issue with max date handling.
        """
        # FIXME: fix problem with max date in past time
        split_data = self.__yahoo_service.get_stock_split_history(ticker.code, '511056000',
                                                                  self.__convert_to_unix_timestamp(datetime.now()))

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker.code, split_data)
            time.sleep(3)

    def scrape_split_for_all_ticker_in_db(self):
        """
        Scrape stock split data for all tickers currently stored in the database.

        This method queries the database for all stock ticker enums of type
        'StockTradeTickers' and processes split data for each one.
        """
        tickers_enum_type_id = self.__stock_repo.get_enum_type('StockTradeTickers')
        enums: List[EnumItem] = self.__stock_repo.get_enums_by_type_id(tickers_enum_type_id)
        for enum in enums:
            self.scrape_split_data(enum)

    def scrape_split_data_to_all_predefined_ticker(self):
        """
        Scrape stock split data for all predefined tickers from the stock list.

        This method processes all tickers defined in the 'stock_to_download' list,
        retrieving and storing their split data.
        """
        for ticker in stock_to_download:
            self.store_split_data(ticker)

    def store_split_data(self, ticker: str):
        """
        Store stock split data for a specific ticker symbol.

        This method retrieves split data from Yahoo Finance and stores it in the database.
        It includes duplicate processing logic (appears to be unintentional).

        Args:
            ticker (str): The stock ticker symbol to process

        Note:
            Contains a FIXME comment about max date handling and appears to have
            duplicate processing logic that should be reviewed.
        """
        # FIXME: fix problem with max date in past time
        split_data = self.__yahoo_service.get_stock_split_history(ticker, '511056000',
                                                                  self.__convert_to_unix_timestamp(datetime.now()))

        if len(split_data) != 0:
            self.__split_scraper.scrape_stocks_splits(ticker, split_data)
            time.sleep(3)
            if len(split_data) != 0:
                self.__split_scraper.scrape_stocks_splits(ticker, split_data)
                time.sleep(3)

    def __convert_to_unix_timestamp(self, date: datetime):
        """
        Convert a datetime object to Unix timestamp.

        This private method converts a Python datetime object to its corresponding
        Unix timestamp (seconds since epoch).

        Args:
            date (datetime): The datetime object to convert

        Returns:
            int: The Unix timestamp representation of the input date
        """
        return int(time.mktime(date.timetuple()))