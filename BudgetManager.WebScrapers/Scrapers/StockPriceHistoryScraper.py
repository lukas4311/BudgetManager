from datetime import datetime
import logging
from influxdb_client import Point, WritePrecision
import csv
from Models.Fmp import StockPriceData
from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton
from secret import alphaVantageToken, influxDbUrl
from Services.AlphaVantage import AlphaVantageService

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizaiton, logging)


class StockPriceScraper:

    def scrape_stocks_prices(self, bucketName: str, ticker: str):
        try:
            alphaVantageService = AlphaVantageService(alphaVantageToken)
            stockPriceData = alphaVantageService.get_stock_price_history(ticker)
            self.__save_price_data_to_influx(bucketName, ticker, stockPriceData)
        except Exception as e:
            logging.info('Error while downloading price for ticker: ' + ticker)
            logging.error(e)

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

        influx_repository.add_range(pointsToSave)
        influx_repository.save_batch(saveAfter=100)


stockPriceScraper = StockPriceScraper()
stockPriceScraper.scrape_stocks_prices('Price', 'ZTS')

# def processTickers(rows):
#     for row in rows:
#         symbol = row["Symbol"]
#
#         try:
#             stockPriceScraper.scrape_stocks_prices('Price', symbol)
#         except Exception:
#             print(symbol + " - error")
#
#
# with open("..\\SourceFiles\\sp500.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     processTickers(csv_file)
