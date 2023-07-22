# importing datetime module
from datetime import datetime, timedelta
import time
import requests
import json
import logging

from Models.FilterTuple import FilterTuple
from Services.InfluxRepository import InfluxRepository
from enum import Enum
from configManager import token, organizaiton
from secret import influxDbUrl

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "StockPrice", token, organizaiton, logging)
bucketName = "Crypto"

class ResultData:
    def __init__(self, data):
        self.data = data


class Result:
    def __init__(self, timestamp, open_val, high_val, low_val, close_val, volume, quoteVolume):
        self.timestamp = timestamp
        self.open_val = open_val
        self.high_val = high_val
        self.low_val = low_val
        self.close_val = close_val
        self.volume = volume
        self.quoteVolume = quoteVolume


class CryptoTickers(Enum):
    BTC = "BTCUSD"
    ETH = "ETHUSD"


class CryptoWatchService:
    oneDayLimit = 86400
    cryptoWatchBaseUrl = "https://api.cryptowat.ch"

    def downloadCryptoPriceHistory(self, ticker: CryptoTickers):
        date_time = datetime.datetime(2000, 7, 26, 21, 20)
        fromTime = int(time.mktime(date_time.timetuple()))
        url = f"{self.cryptoWatchBaseUrl}/markets/coinbase-pro/{ticker.value}/ohlc?periods={self.oneDayLimit}&after={fromTime}"
        print(url)

        lastRecordTime = self.get_last_record_time(ticker)

        #now_datetime_with_offset = datetime.now().astimezone(last_downloaded_time.tzinfo) - timedelta(days=1)

        # if last_downloaded_time < now_datetime_with_offset:
        #     stockPriceData = self.__scrape_stock_data(ticker, last_downloaded_time, date_to)
        #     stockPriceData = [d for d in stockPriceData if d.date > last_downloaded_time]

        # response = requests.get(url)
        # jsonData = response.text
        # parsed_data = json.loads(jsonData)
        #
        # result_objects = [Result(*item) for item in parsed_data['result']['86400']]
        # result_data_instance = ResultData(result_objects)
        #
        # for result in result_data_instance.data:
        #     print(f"Timestamp: {result.timestamp}, Close Value: {result.close_val}")

    def get_last_record_time(self, ticker: CryptoTickers):
        lastValue = influx_repository.filter_last_value(bucketName, FilterTuple("ticker", ticker), datetime.min)
        last_downloaded_time = datetime(2000, 1, 1)

        if len(lastValue) != 0:
            last_downloaded_time = lastValue[0].records[0]["_time"]

        return last_downloaded_time


cryptoService = CryptoWatchService();
cryptoService.downloadCryptoPriceHistory(CryptoTickers.BTC)