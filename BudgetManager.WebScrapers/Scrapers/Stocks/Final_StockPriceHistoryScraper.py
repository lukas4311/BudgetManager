import json
from datetime import datetime, timedelta
import logging
from typing import List

from dacite import from_dict
from influxdb_client import Point, WritePrecision
import time

from Models.FilterTuple import FilterTuple
from Models.Fmp import StockPriceData
from Services.DB.Orm.EnumItem import EnumItem
from Scrapers.TradingViewScraper import TickerMetadata
from Services.DB.StockRepository import StockRepository
from Services.InfluxRepository import InfluxRepository
from Services.YahooService import YahooService
from SourceFiles.stockList import stock_to_download
from secret import token, organizationId
from config import influxUrl

log_name = './Scrapers/Stocks/Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxUrl, "StockPrice", token, organizationId, logging)


class StockPriceScraper:
    """
    Core scraping engine for individual stock price data.

    This class handles the scraping of price data for a single stock ticker,
    including incremental updates, data transformation, and storage to InfluxDB.
    """

    influx_repo: InfluxRepository = None

    def __init__(self, influx_repo: InfluxRepository):
        """
        Initialize the stock price scraper with an InfluxDB repository.

        Args:
            influx_repo: InfluxDB repository instance for data storage
        """
        self.influx_repo = influx_repo

    def scrape_stocks_prices(self, measurement: str, ticker: str, tag: str):
        """
        Scrape and save stock price data for a specific ticker.

        This method performs incremental data collection by checking the last
        recorded timestamp in InfluxDB and only fetching new data since then.

        Args:
            measurement: InfluxDB measurement name for storing the data
            ticker: Stock ticker symbol (e.g., 'AAPL', 'GOOGL')
            tag: Tag value used for filtering in InfluxDB (typically same as ticker)
        """
        try:
            stock_price_data: list[StockPriceData] = []
            date_to = datetime.now()
            last_value = self.influx_repo.filter_last_value(measurement, FilterTuple("ticker", tag), datetime.min)

            if len(last_value) != 0:
                last_downloaded_time = last_value[0].records[0]["_time"]
                now_datetime_with_offset = datetime.now().astimezone(last_downloaded_time.tzinfo) - timedelta(days=1)

                if last_downloaded_time < now_datetime_with_offset:
                    stock_price_data = self.__scrape_stock_data(ticker, last_downloaded_time, date_to)
                    stock_price_data = [d for d in stock_price_data if d.date > last_downloaded_time]
            else:
                stock_price_data = self.__scrape_stock_data(ticker, None, date_to)

            self.__save_price_data_to_influx(measurement, tag, stock_price_data)
        except Exception as e:
            logging.info('Error while downloading price for ticker: ' + ticker)
            logging.error(e)

    def __scrape_stock_data(self, ticker: str, date_from: datetime, date_to: datetime):
        """
        Fetch stock price data from Yahoo Finance API.

        This method converts datetime objects to Unix timestamps and calls
        the Yahoo Finance service to retrieve historical price data.

        Args:
            ticker: Stock ticker symbol
            date_from: Start date for data retrieval (None for all available data)
            date_to: End date for data retrieval

        Returns:
            list[StockPriceData]: List of stock price data points
        """
        yahoo_service = YahooService()
        unix_from = '511056000' if date_from is None else str(
            self.__convert_to_unix_timestamp(date_from + timedelta(days=1)))
        unix_to = str(self.__convert_to_unix_timestamp(date_to))
        return yahoo_service.get_stock_price_history_new(ticker, unix_from, unix_to)

    def __save_price_data_to_influx(self, measurement: str, ticker: str, priceData: list):
        """
        Save stock price data to InfluxDB.

        This method converts StockPriceData objects to InfluxDB Points
        and writes them to the database in batch format.

        Args:
            measurement: InfluxDB measurement name
            ticker: Stock ticker for tagging the data points
            priceData: List of StockPriceData objects to save
        """
        price_model: StockPriceData
        points_to_save = []
        logging.info('Saving price for stock: ' + ticker)
        for price_model in priceData:
            point = Point(measurement) \
                .tag("ticker", ticker) \
                .field('price', price_model.value)
            point = point.time(price_model.date, WritePrecision.NS)
            points_to_save.append(point)

        print(len(points_to_save))
        self.influx_repo.add_range(points_to_save)
        self.influx_repo.save()

    def __convert_to_unix_timestamp(self, date: datetime):
        """
        Convert a datetime object to Unix timestamp.

        Args:
            date: Datetime object to convert

        Returns:
            int: Unix timestamp as integer
        """
        return int(time.mktime(date.timetuple()))


class StockPriceManager:
    """
    Manager class for coordinating stock price scraping operations.

    This class provides high-level methods for scraping multiple stocks,
    managing delays between requests, and handling different data sources
    including predefined lists and database-driven enum items.
    """

    def __init__(self, stock_repo: StockRepository):
        """
        Initialize the stock price manager.

        Args:
            stock_repo: Repository for accessing stock-related database operations
        """
        self.__stock_repo = stock_repo
        self.__stockPriceScraper = StockPriceScraper(influx_repository)

    def scrape_ticker_price(self, ticker: str, delay=0):
        """
        Scrape price data for a single ticker with optional delay.

        This method scrapes data for one stock ticker and optionally
        waits for a specified delay period to respect API rate limits.

        Args:
            ticker: Stock ticker symbol to scrape
            delay: Number of seconds to wait after scraping (default: 0)
        """
        message = 'Loading data for ' + ticker
        print(message)
        logging.info(message)

        try:
            self.__stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)
        except Exception:
            influx_repository.clear()
            print(ticker + " - error")

        print(f'Sleeping for {delay} seconds')
        time.sleep(delay)
        print("Sleeping is done.")

    def scrape_tickers_price(self, delay=0):
        """
        Scrape price data for all tickers in the predefined stock list.

        This method iterates through all tickers in the stock_to_download list
        and scrapes price data for each one with a configurable delay between requests.

        Args:
            delay: Number of seconds to wait between each ticker scraping (default: 0)
        """
        for ticker in stock_to_download:
            message = 'Loading data for ' + ticker
            print(message)
            logging.info(message)

            try:
                self.__stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)
            except Exception:
                influx_repository.clear_entities()
                print(ticker + " - error")

            print(f'Sleeping for {delay} seconds')
            time.sleep(delay)
            print("Sleeping is done.")

    def scrape_all_enum_item_tickers(self, delay: int = 0):
        """
        Scrape price data for all tickers defined in the database enum items.

        This method retrieves ticker information from the database enum system,
        which allows for dynamic ticker management through the database. It supports
        custom ticker mappings via metadata for cases where the display ticker
        differs from the price ticker.

        Args:
            delay: Number of seconds to wait between each ticker scraping (default: 0)
        """
        tickers_enum_type_id = self.__stock_repo.get_enum_type('StockTradeTickers')
        enums: List[EnumItem] = self.__stock_repo.get_enums_by_type_id(tickers_enum_type_id)

        for ticker in enums:
            message = 'Loading data for ' + ticker.code
            print(message)
            logging.info(message)

            try:
                db_metadata = json.loads(ticker._metadata)
                db_metadata_model = from_dict(TickerMetadata, db_metadata)
                ticker_to_scrape_price = ticker.code if db_metadata_model.price_ticker is None else db_metadata_model.price_ticker
                self.__stockPriceScraper.scrape_stocks_prices('Price', ticker_to_scrape_price, ticker_to_scrape_price)
            except Exception:
                influx_repository.clear_entities()
                print(ticker.code + " - error")

            print(f'Sleeping for {delay} seconds')
            time.sleep(delay)
            print("Sleeping is done.")

#
# tickersToScrape = stockToDownload
# stockPriceScraper = StockPriceScraper(influx_repository)
#
# for ticker in tickersToScrape:
#      stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)
# pricescraper = StockPriceManager(StockRepository())
# pricescraper.scrape_all_enum_item_tickers(1)