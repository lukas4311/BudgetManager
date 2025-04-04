import json
from datetime import datetime, timedelta
import logging
from typing import List

from dacite import from_dict
from influxdb_client import Point, WritePrecision
import time

from Models.FilterTuple import FilterTuple
from Models.Fmp import StockPriceData
from Orm.EnumItem import EnumItem
from Scrapers.TradingViewScraper import TickerMetadata
from Services.DB.StockRepository import StockRepository
from Services.InfluxRepository import InfluxRepository
from Services.YahooService import YahooService
from SourceFiles.stockList import stockToDownload
from secret import token, organizationId
from secret import influxDbUrl

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizationId, logging)


class StockPriceScraper:
    influx_repo: InfluxRepository = None

    def __init__(self, influx_repo: InfluxRepository):
        self.influx_repo = influx_repo

    def scrape_stocks_prices(self, measurement: str, ticker: str, tag: str):
        try:
            stockPriceData: list[StockPriceData] = []
            date_to = datetime.now()
            lastValue = self.influx_repo.filter_last_value(measurement, FilterTuple("ticker", tag), datetime.min)

            if len(lastValue) != 0:
                last_downloaded_time = lastValue[0].records[0]["_time"]
                now_datetime_with_offset = datetime.now().astimezone(last_downloaded_time.tzinfo) - timedelta(days=1)

                if last_downloaded_time < now_datetime_with_offset:
                    stockPriceData = self.__scrape_stock_data(ticker, last_downloaded_time, date_to)
                    stockPriceData = [d for d in stockPriceData if d.date > last_downloaded_time]
            else:
                stockPriceData = self.__scrape_stock_data(ticker, None, date_to)

            self.__save_price_data_to_influx(measurement, tag, stockPriceData)
        except Exception as e:
            logging.info('Error while downloading price for ticker: ' + ticker)
            logging.error(e)

    def __scrape_stock_data(self, ticker: str, date_from: datetime, date_to: datetime):
        yahooService = YahooService()
        unix_from = '511056000' if date_from is None else str(
            self.__convert_to_unix_timestamp(date_from + timedelta(days=1)))
        unix_to = str(self.__convert_to_unix_timestamp(date_to))
        return yahooService.get_stock_price_history_new(ticker, unix_from, unix_to)

    def __save_price_data_to_influx(self, measurement: str, ticker: str, priceData: list):
        priceModel: StockPriceData
        pointsToSave = []
        logging.info('Saving price for stock: ' + ticker)
        for priceModel in priceData:
            point = Point(measurement) \
                .tag("ticker", ticker) \
                .field('price', priceModel.value)
            point = point.time(priceModel.date, WritePrecision.NS)
            pointsToSave.append(point)

        print(len(pointsToSave))
        self.influx_repo.add_range(pointsToSave)
        self.influx_repo.save()

    def __convert_to_unix_timestamp(self, date: datetime):
        return int(time.mktime(date.timetuple()))


class StockPriceManager:
    def __init__(self, stock_repo: StockRepository):
        self.__stock_repo = stock_repo
        self.__stockPriceScraper = StockPriceScraper(influx_repository)

    def scrape_ticker_price(self, ticker: str, delay=0):
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
        for ticker in stockToDownload:
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
