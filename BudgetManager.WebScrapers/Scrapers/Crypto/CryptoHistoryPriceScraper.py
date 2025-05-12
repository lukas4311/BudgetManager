# importing datetime module
from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
import time
import requests
import json
import logging

from Models.FilterTuple import FilterTuple
from Services.InfluxRepository import InfluxRepository
from enum import Enum
from secret import token, organizationId
from config import influxUrl
from influxdb_client import Point, WritePrecision
from typing import List

log_name = 'Logs/cryptoPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxUrl, "CryptoV2", token, organizationId, logging)
measurement = "Price"
coinbaseExchange = "coinbase-pro"
geminiExchange = "gemini"


@dataclass
class CryptoPriceData:
    date: datetime
    price: float
    ticker: str


class ResultData:
    def __init__(self, data):
        self.data = data


class Result:
    def __init__(self, timestamp, open_val, high_val, low_val, close_val, vwap, volume, count):
        self.timestamp = timestamp
        self.open_val = open_val
        self.high_val = high_val
        self.low_val = low_val
        self.close_val = close_val
        self.vwap = vwap
        self.volume = volume
        self.count = count


class CryptoTickers(Enum):
    XXBTZUSD = "XXBTZUSD"
    XETHZUSD = "XETHZUSD"
    MATICUSD = "MATICUSD"
    LINKUSD = "LINKUSD"
    SNXUSD = "SNXUSD"
    USDCUSD = "USDCUSD"


class CryptoTickerTranslator:
    def translate(self, crypto_ticker: CryptoTickers):
        match crypto_ticker:
            case CryptoTickers.XXBTZUSD:
                return "BTC"
            case CryptoTickers.XETHZUSD:
                return "ETH"
            case CryptoTickers.MATICUSD:
                return "MATIC"
            case CryptoTickers.LINKUSD:
                return "LINK"
            case CryptoTickers.SNXUSD:
                return "SNX"
            case CryptoTickers.USDCUSD:
                return "USDC"


class CryptoWatchService:
    one_day_limit = 1440
    crypto_watch_base_url = "https://api.kraken.com/0/public"

    def get_crypto_price_history(self, ticker: CryptoTickers):
        crypto_ticker_translator = CryptoTickerTranslator()
        translated_ticker = crypto_ticker_translator.translate(ticker)
        last_record_time = self.__get_last_record_time(translated_ticker)
        now_datetime_with_offset = datetime.now().astimezone(last_record_time.tzinfo)

        if last_record_time < now_datetime_with_offset:
            fromTime = int(time.mktime(last_record_time.timetuple()))
            # exchange = geminiExchange if ticker == CryptoTickers.USDC else coinbaseExchange
            url = f"{self.crypto_watch_base_url}/OHLC?pair={ticker.value}&interval={self.one_day_limit}&since={fromTime}"
            print(url)
            response = requests.get(url)
            json_data = response.text
            parsed_data = json.loads(json_data)
            print(parsed_data)
            result_objects = [Result(*item) for item in parsed_data['result'][ticker.value]]
            result_data_instance = ResultData(result_objects)
            stockPriceData = [CryptoPriceData(datetime.fromtimestamp(d.timestamp).astimezone(timezone.utc) - timedelta(hours=1), float(round(float(d.close_val), 2)), translated_ticker) for d in result_data_instance.data if d.timestamp > fromTime]

            return stockPriceData

        return []

    def __get_last_record_time(self, ticker: str):
        last_value = influx_repository.filter_last_value(measurement, FilterTuple("ticker", ticker), datetime.min)
        print(ticker)
        print(last_value)
        last_downloaded_time = datetime(1975, 1, 1, 0, 0, 0, tzinfo=timezone.utc)

        if len(last_value) != 0:
            last_downloaded_time = last_value[0].records[0]["_time"]

        return last_downloaded_time

    def save_data_to_influx(self, priceData: List[CryptoPriceData]):
        points_to_save = []
        logging.info('Saving price for stock: ' + priceData[0].ticker)

        for price_model in priceData:
            point = Point(measurement) \
                .tag("ticker", price_model.ticker) \
                .field('price', price_model.price)
            point = point.time(price_model.date, WritePrecision.NS)
            points_to_save.append(point)

        influx_repository.add_range(points_to_save)

        for point in points_to_save:
            print(point.to_line_protocol())

        influx_repository.save()
        logging.info('Data saved for ticker: ' + priceData[0].ticker)
        print("Data saved")


class CryptoPriceManager:
    def scrape_crypto_price(self):
        crypto_service = CryptoWatchService()

        btc_data = crypto_service.get_crypto_price_history(CryptoTickers.XXBTZUSD)
        print(btc_data)
        if len(btc_data) > 0:
            crypto_service.save_data_to_influx(btc_data)

        eth_data = crypto_service.get_crypto_price_history(CryptoTickers.XETHZUSD)
        print(eth_data)
        if len(eth_data) > 0:
            crypto_service.save_data_to_influx(eth_data)

        link = crypto_service.get_crypto_price_history(CryptoTickers.LINKUSD)
        if len(link) > 0:
            crypto_service.save_data_to_influx(link)

        matic = crypto_service.get_crypto_price_history(CryptoTickers.MATICUSD)
        if len(matic) > 0:
            crypto_service.save_data_to_influx(matic)

        snx = crypto_service.get_crypto_price_history(CryptoTickers.SNXUSD)
        if len(snx) > 0:
            crypto_service.save_data_to_influx(snx)

        usdc = crypto_service.get_crypto_price_history(CryptoTickers.USDCUSD)
        if len(snx) > 0:
            crypto_service.save_data_to_influx(usdc)