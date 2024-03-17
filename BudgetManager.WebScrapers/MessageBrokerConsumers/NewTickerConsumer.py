from datetime import datetime
import logging
import json

from Scrapers.StockFinacialAndCompanyData import StockScrapeManager
from Scrapers.Stocks.Final_StockPriceHistoryScraper import StockPriceScraper
from Scrapers.Stocks.Final_StockSplitHistoryScraper import StockSplitManager
from Scrapers.FmpApi import FmpScraper
from Services.FmpApiService import FmpApiService
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService
from secret import influxDbUrl, organizationId, token, fmpApiToken

log_name = 'Logs/newTickerRequest.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class StockTickerManager:
    def __init__(self):
        self.__influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizationId, logging)
        self.__influx_repository_Roic = InfluxRepository(influxDbUrl, "StocksRoic", token, organizationId, logging)
        self.__stockPriceScraper = StockPriceScraper(self.__influx_repository)
        self.__roic_service = RoicService()
        self.__fmpScraper = FmpScraper()
        self.__stock_scraper = StockScrapeManager(self.__roic_service, self.__fmpScraper, self.__influx_repository_Roic)
        self.__fmp_service = FmpApiService(fmpApiToken)

    def store_new_ticker_info(self, ticker: str):
        print("Store ticker")
        profile = self.__fmp_service.get_company_profile(ticker)
        company_Name = profile.companyName
        self.__stock_scraper.storeTickers(ticker, company_Name)

        print("Storing stock price")
        self.__stockPriceScraper.scrape_stocks_prices('Price', ticker, ticker)

        print("Storing stock split")
        stock_split_manager = StockSplitManager()
        stock_split_manager.store_split_data(ticker)

        print("Storing company profile")
        self.__fmpScraper.download_profile(ticker)

        # TODO: temporarily commented cause missing roic stock bucket missing
        # print("Storing company fin. info")
        # self.__stock_scraper.download_main_fin(ticker)
        #
        # print("Storing company financial summary")
        # self.__stock_scraper.download_fin_summary(ticker)


def receive_new_ticker(ch, method, properties, body):
    print(f" [x] Received {body}")
    process_ticker_from_message_queue(body)


def process_ticker_from_message_queue(msg: str):
    message_object = json.loads(msg)

    try:
        ticker = message_object['message']['ticker']
        stock_ticker_manager = StockTickerManager
        stock_ticker_manager.store_new_ticker_info(ticker)
    except:
        print('Error while parsing ticker from message')


stockTickerManager = StockTickerManager()
stockTickerManager.store_new_ticker_info("PLTR")

exit()

queue_name = 'test'
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()
channel.queue_declare(queue=queue_name, durable=True)
channel.basic_consume(queue=queue_name, on_message_callback=receive_new_ticker, auto_ack=True)

print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()
