from datetime import datetime
import logging

import pika

from Scrapers.StockFinacialAndCompanyData import StockScrapeManager
from Scrapers.Stocks.Final_StockPriceHistoryScraper import StockPriceScraper
from Scrapers.Stocks.Final_StockSplitHistoryScraper import StockSplitManager
from Scrapers.FmpApi import FmpScraper
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService
from secret import influxDbUrl, organizationId, token

log_name = 'Logs/newTickerRequest.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


def receive_new_ticker(ch, method, properties, body):
    print(f" [x] Received {body}")


queue_name = 'test'
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()
channel.queue_declare(queue=queue_name, durable=True)
channel.basic_consume(queue=queue_name, on_message_callback=receive_new_ticker, auto_ack=True)

print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()


class StockTickerManager:

    def __init__(self):
        self.__influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizationId, logging)
        self.__influx_repository_Roic = InfluxRepository(influxDbUrl, "StocksRoic", token, organizationId, logging)
        self.__stockPriceScraper = StockPriceScraper(self.__influx_repository)
        self.__roic_service = RoicService()
        self.__fmpScraper = FmpScraper()
        self.__stock_scraper = StockScrapeManager(self.__roic_service, self.__fmpScraper, self.__influx_repository_Roic)

    def store_new_ticker_info(self, ticker: str):
        print("Store ticker")
        company_Name = ticker
        # FIXME: need to get company name for ticker
        self.__stock_scraper.storeTickers(ticker, company_Name)

        print("Storing stock price")
        self.__stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)

        print("Storing stock split")
        stock_split_manager = StockSplitManager()
        stock_split_manager.store_split_data(ticker)

        print("Storing company info")
        fmp_scraper = FmpScraper()
        fmp_scraper.download_profile(ticker)

        print("Storing company profile")
        fmpScraper = FmpScraper()
        fmpScraper.download_profile(ticker)

        print("Storing company fin. info")
        self.__stock_scraper.download_main_fin(ticker)