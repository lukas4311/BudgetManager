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
from secret import influxDbUrl
from influxdb_client import Point, WritePrecision
from typing import List

log_name = 'Logs/cryptoPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "CryptoV2", token, organizationId, logging)
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
    oneDayLimit = 1440
    cryptoWatchBaseUrl = "https://api.kraken.com/0/public"

    def get_crypto_price_history(self, ticker: CryptoTickers):
        cryptoTickerTranslator = CryptoTickerTranslator()
        translated_ticker = cryptoTickerTranslator.translate(ticker)
        lastRecordTime = self.__get_last_record_time(translated_ticker)
        now_datetime_with_offset = datetime.now().astimezone(lastRecordTime.tzinfo)

        if lastRecordTime < now_datetime_with_offset:
            fromTime = int(time.mktime(lastRecordTime.timetuple()))
            # exchange = geminiExchange if ticker == CryptoTickers.USDC else coinbaseExchange
            url = f"{self.cryptoWatchBaseUrl}/OHLC?pair={ticker.value}&interval={self.oneDayLimit}&since={fromTime}"
            print(url)
            response = requests.get(url)
            jsonData = response.text
            parsed_data = json.loads(jsonData)
            print(parsed_data)
            result_objects = [Result(*item) for item in parsed_data['result'][ticker.value]]
            result_data_instance = ResultData(result_objects)
            stockPriceData = [CryptoPriceData(datetime.fromtimestamp(d.timestamp).astimezone(timezone.utc) - timedelta(hours=1), float(round(float(d.close_val), 2)), translated_ticker) for d in result_data_instance.data if d.timestamp > fromTime]

            return stockPriceData

        return []

    def __get_last_record_time(self, ticker: str):
        lastValue = influx_repository.filter_last_value(measurement, FilterTuple("ticker", ticker), datetime.min)
        print(ticker)
        print(lastValue)
        last_downloaded_time = datetime(1975, 1, 1, 0, 0, 0, tzinfo=timezone.utc)

        if len(lastValue) != 0:
            last_downloaded_time = lastValue[0].records[0]["_time"]

        return last_downloaded_time

    def save_data_to_influx(self, priceData: List[CryptoPriceData]):
        pointsToSave = []
        logging.info('Saving price for stock: ' + priceData[0].ticker)

        for priceModel in priceData:
            point = Point(measurement) \
                .tag("ticker", priceModel.ticker) \
                .field('price', priceModel.price)
            point = point.time(priceModel.date, WritePrecision.NS)
            pointsToSave.append(point)

        influx_repository.add_range(pointsToSave)

        for point in pointsToSave:
            print(point.to_line_protocol())

        influx_repository.save()
        logging.info('Data saved for ticker: ' + priceData[0].ticker)
        print("Data saved")


class CryptoPriceManager:
    def scrape_crypto_price(self):
        cryptoService = CryptoWatchService()

        btcData = cryptoService.get_crypto_price_history(CryptoTickers.XXBTZUSD)
        print(btcData)
        if len(btcData) > 0:
            cryptoService.save_data_to_influx(btcData)

        ethData = cryptoService.get_crypto_price_history(CryptoTickers.XETHZUSD)
        print(ethData)
        if len(ethData) > 0:
            cryptoService.save_data_to_influx(ethData)

        link = cryptoService.get_crypto_price_history(CryptoTickers.LINKUSD)
        if len(link) > 0:
            cryptoService.save_data_to_influx(link)

        matic = cryptoService.get_crypto_price_history(CryptoTickers.MATICUSD)
        if len(matic) > 0:
            cryptoService.save_data_to_influx(matic)

        snx = cryptoService.get_crypto_price_history(CryptoTickers.SNXUSD)
        if len(snx) > 0:
            cryptoService.save_data_to_influx(snx)

        usdc = cryptoService.get_crypto_price_history(CryptoTickers.USDCUSD)
        if len(snx) > 0:
            cryptoService.save_data_to_influx(usdc)