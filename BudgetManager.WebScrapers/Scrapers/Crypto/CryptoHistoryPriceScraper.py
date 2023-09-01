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


@dataclass
class CryptoPriceData:
    date: datetime
    price: float
    ticker: str


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


class CryptoTickerTranslator:
    def translate(self, crypto_ticker: CryptoTickers):
        match crypto_ticker:
            case CryptoTickers.BTC:
                return "BTC"
            case CryptoTickers.ETH:
                return "ETH"


class CryptoWatchService:
    oneDayLimit = 86400
    cryptoWatchBaseUrl = "https://api.cryptowat.ch"

    def get_crypto_price_history(self, ticker: CryptoTickers):
        cryptoTickerTranslator = CryptoTickerTranslator()
        translated_ticker = cryptoTickerTranslator.translate(ticker)
        lastRecordTime = self.__get_last_record_time(translated_ticker)
        now_datetime_with_offset = datetime.now().astimezone(lastRecordTime.tzinfo)

        if lastRecordTime < now_datetime_with_offset:
            fromTime = int(time.mktime(lastRecordTime.timetuple()))
            url = f"{self.cryptoWatchBaseUrl}/markets/coinbase-pro/{ticker.value}/ohlc?periods={self.oneDayLimit}&after={fromTime}"
            print(url)
            response = requests.get(url)
            jsonData = response.text
            parsed_data = json.loads(jsonData)
            result_objects = [Result(*item) for item in parsed_data['result']['86400']]
            result_data_instance = ResultData(result_objects)
            stockPriceData = [CryptoPriceData(datetime.fromtimestamp(d.timestamp).astimezone(timezone.utc) - timedelta(hours=1), float(round(d.close_val, 2)), translated_ticker) for d in result_data_instance.data if d.timestamp > fromTime]

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


cryptoService = CryptoWatchService()
btcData = cryptoService.get_crypto_price_history(CryptoTickers.BTC)

if len(btcData) > 0:
    cryptoService.save_data_to_influx(btcData)

ethData = cryptoService.get_crypto_price_history(CryptoTickers.ETH)

if len(ethData) > 0:
    cryptoService.save_data_to_influx(ethData)
