from datetime import datetime
import logging
import json

import pika

from Orm.Notification import Notification
from Scrapers.StockFinacialAndCompanyData import StockScrapeManager
from Scrapers.Stocks.Final_StockPriceHistoryScraper import StockPriceScraper
from Scrapers.Stocks.Final_StockSplitHistoryScraper import StockSplitManager
from Scrapers.FmpApi import FmpScraper
from Scrapers.TradingViewScraper import TradingviewScraper
from Services.DB.NotificationRepository import NotificationRepository
from Services.DB.StockRepository import StockRepository
from Services.FmpApiService import FmpApiService
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService
from Services.StockService import StockService
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

        stock_service = StockService(StockRepository(), TradingviewScraper())
        stock_service.check_tickers_metadata()

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

        self.__send_notification()

    def process_ticker_from_message_queue(self, msg: str):
        message_object = json.loads(msg)
        print(message_object)

        try:
            print(message_object['message'])
            ticker = message_object['message']['ticker']
            user_id = message_object['message']['userId']
            stock_ticker_manager = StockTickerManager()
            stock_ticker_manager.store_new_ticker_info(ticker)
            self.__send_notification(ticker, user_id)
        except Exception as e:
            logging.info('Error while saving data for new ticker: ' + ticker)
            logging.error(e)


    def __send_notification(self, ticker: str, user_id: int):
        notify_repo = NotificationRepository()
        notification = Notification()
        notification.userIdentityId = user_id
        notification.heading = 'Ticker were added'
        notification.content = f'Your request for {ticker} was completed.'
        notification.isDisplayed = False
        notification.timestamp = datetime.now()
        notification.attachmentUrl = None
        notify_repo.insert_stock_trade(notification)

def receive_new_ticker(ch, method, properties, body):
    print(f" [x] Received {body}")
    stock_ticker_Manager = StockTickerManager()
    stock_ticker_Manager.process_ticker_from_message_queue(body)

queue_name = 'test'
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()
channel.queue_declare(queue=queue_name, durable=True)
channel.basic_consume(queue=queue_name, on_message_callback=receive_new_ticker, auto_ack=True)

print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()
