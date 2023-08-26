from datetime import datetime, timedelta
import logging
from influxdb_client import Point, WritePrecision
import csv
import time

from Models.FilterTuple import FilterTuple
from Models.Fmp import StockPriceData
from Services.InfluxRepository import InfluxRepository
from Services.YahooService import YahooService
from SourceFiles.stockList import stockToDownload
from configManager import token, organizaiton
from secret import influxDbUrl

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizaiton, logging)


class StockPriceScraper:
    influx_repo:InfluxRepository = None

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
        return yahooService.get_stock_price_history(ticker, unix_from, unix_to)

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

        # print(pointsToSave)
        self.influx_repo.add_range(pointsToSave)
        self.influx_repo.save()

    def __convert_to_unix_timestamp(self, date: datetime):
        return int(time.mktime(date.timetuple()))


tickersToScrape = stockToDownload
stockPriceScraper = StockPriceScraper(influx_repository)

def processTickers(rows):
    for row in rows:
        symbol = row["Symbol"]
        message = 'Loading data for ' + symbol
        print(message)
        logging.info(message)

        try:
            stockPriceScraper.scrape_stocks_prices('Price', symbol, symbol)
        except Exception:
            influx_repository.clear()
            print(symbol + " - error")

        print("Sleeping for 2 seconds")
        time.sleep(2)
        print("Sleeping is done.")


# for ticker in tickersToScrape:
#     stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)

# with open("..\\SourceFiles\\sp500.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     processTickers(csv_file)
