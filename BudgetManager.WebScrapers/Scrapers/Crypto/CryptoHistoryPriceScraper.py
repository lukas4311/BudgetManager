from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
import time
import requests
import json
import logging

from Models.FilterTuple import FilterTuple
from Scrapers.Crypto.CryptoTickerTranslator import CryptoTickerTranslator
from Services.InfluxRepository import InfluxRepository
from enum import Enum
from secret import token, organizationId
from config import influxUrl, crypto_watch_base_url
from influxdb_client import Point, WritePrecision
from typing import List

# Configure logging with daily rotation
log_name = 'Logs/cryptoPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)

# Initialize InfluxDB repository
influx_repository = InfluxRepository(influxUrl, "CryptoV2", token, organizationId, logging)
measurement = "Price"

# Exchange identifiers (currently unused but may be useful for future multi-exchange support)
coinbaseExchange = "coinbase-pro"
geminiExchange = "gemini"


@dataclass
class CryptoPriceData:
    """
    Represents a single cryptocurrency price data point.

    Attributes:
        date (datetime): The timestamp of the price data
        price (float): The price value in USD
        ticker (str): The cryptocurrency ticker symbol (e.g., 'BTC', 'ETH')
    """
    date: datetime
    price: float
    ticker: str


class ResultData:
    """
    Container class for multiple Result objects from API response.

    Attributes:
        data: List of Result objects containing OHLC data
    """

    def __init__(self, data):
        self.data = data


class Result:
    """
    Represents OHLC (Open, High, Low, Close) data for a single time period.

    This class maps directly to the Kraken API OHLC response format.

    Attributes:
        timestamp: Unix timestamp of the data point
        open_val: Opening price
        high_val: Highest price during the period
        low_val: Lowest price during the period
        close_val: Closing price
        vwap: Volume-weighted average price
        volume: Trading volume
        count: Number of trades
    """

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
    """
    Enumeration of supported cryptocurrency trading pairs from Kraken.

    These represent the exact ticker symbols used by the Kraken API.
    """
    XXBTZUSD = "XXBTZUSD"  # Bitcoin to USD
    XETHZUSD = "XETHZUSD"  # Ethereum to USD
    MATICUSD = "MATICUSD"  # Polygon (MATIC) to USD
    LINKUSD = "LINKUSD"  # Chainlink to USD
    SNXUSD = "SNXUSD"  # Synthetix to USD
    USDCUSD = "USDCUSD"  # USD Coin to USD


class CryptoWatchService:
    """
    Service class for interacting with the Kraken API to fetch cryptocurrency price data.

    This service handles:
    - Fetching historical OHLC data from Kraken
    - Converting the data to internal format
    - Saving data to InfluxDB
    - Incremental updates based on last recorded timestamp
    """

    # Class constants
    one_day_limit = 1440  # Minutes in a day - used for daily OHLC intervals

    def get_crypto_price_history(self, ticker: CryptoTickers) -> List[CryptoPriceData]:
        """
        Fetch historical price data for a specific cryptocurrency.

        This method retrieves OHLC data from Kraken API starting from the last
        recorded timestamp in the database to avoid duplicate data.

        Args:
            ticker (CryptoTickers): The cryptocurrency ticker to fetch data for

        Returns:
            List[CryptoPriceData]: List of price data points, empty if no new data
        """
        crypto_ticker_translator = CryptoTickerTranslator()
        translated_ticker = crypto_ticker_translator.translate(ticker)
        last_record_time = self.__get_last_record_time(translated_ticker)
        now_datetime_with_offset = datetime.now().astimezone(last_record_time.tzinfo)

        if last_record_time < now_datetime_with_offset:
            fromTime = int(time.mktime(last_record_time.timetuple()))

            # exchange = geminiExchange if ticker == CryptoTickers.USDC else coinbaseExchange
            url = f"{crypto_watch_base_url}/OHLC?pair={ticker.value}&interval={self.one_day_limit}&since={fromTime}"
            print(url)

            response = requests.get(url)
            json_data = response.text
            parsed_data = json.loads(json_data)
            print(parsed_data)

            result_objects = [Result(*item) for item in parsed_data['result'][ticker.value]]
            result_data_instance = ResultData(result_objects)

            stockPriceData = [
                CryptoPriceData(
                    datetime.fromtimestamp(d.timestamp).astimezone(timezone.utc) - timedelta(hours=1),
                    float(round(float(d.close_val), 2)),
                    translated_ticker
                )
                for d in result_data_instance.data
                if d.timestamp > fromTime
            ]

            return stockPriceData

        return []

    def __get_last_record_time(self, ticker: str) -> datetime:
        """
        Get the timestamp of the last recorded data point for a given ticker.

        Args:
            ticker (str): The simplified ticker symbol (e.g., 'BTC')

        Returns:
            datetime: The timestamp of the last record, or a default date if no records exist
        """
        last_value = influx_repository.filter_last_value(measurement, FilterTuple("ticker", ticker), datetime.min)
        print(ticker)
        print(last_value)

        last_downloaded_time = datetime(1975, 1, 1, 0, 0, 0, tzinfo=timezone.utc)

        if len(last_value) != 0:
            last_downloaded_time = last_value[0].records[0]["_time"]

        return last_downloaded_time

    def save_data_to_influx(self, priceData: List[CryptoPriceData]) -> None:
        """
        Save cryptocurrency price data to InfluxDB.

        This method converts CryptoPriceData objects to InfluxDB Points
        and writes them to the database in batch.

        Args:
            priceData (List[CryptoPriceData]): List of price data to save
        """
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
    """
    Main orchestrator class for cryptocurrency price scraping.

    This class coordinates the scraping process for all supported cryptocurrencies,
    fetching data for each ticker and saving it to the database.
    """

    def scrape_crypto_price(self) -> None:
        """
        Main method to scrape price data for all supported cryptocurrencies.

        This method iterates through all supported tickers, fetches new price data,
        and saves it to InfluxDB. Only processes data if new records are available
        to avoid unnecessary API calls and duplicate data.
        """
        crypto_service = CryptoWatchService()

        # Process Bitcoin (BTC)
        btc_data = crypto_service.get_crypto_price_history(CryptoTickers.XXBTZUSD)
        print(btc_data)
        if len(btc_data) > 0:
            crypto_service.save_data_to_influx(btc_data)

        # Process Ethereum (ETH)
        eth_data = crypto_service.get_crypto_price_history(CryptoTickers.XETHZUSD)
        print(eth_data)
        if len(eth_data) > 0:
            crypto_service.save_data_to_influx(eth_data)

        # Process Chainlink (LINK)
        link = crypto_service.get_crypto_price_history(CryptoTickers.LINKUSD)
        if len(link) > 0:
            crypto_service.save_data_to_influx(link)

        # Process Polygon/Matic (MATIC)
        matic = crypto_service.get_crypto_price_history(CryptoTickers.MATICUSD)
        if len(matic) > 0:
            crypto_service.save_data_to_influx(matic)

        # Process Synthetix (SNX)
        snx = crypto_service.get_crypto_price_history(CryptoTickers.SNXUSD)
        if len(snx) > 0:
            crypto_service.save_data_to_influx(snx)

        # Process USD Coin (USDC)
        usdc = crypto_service.get_crypto_price_history(CryptoTickers.USDCUSD)
        # Bug Fix: The condition should check 'usdc' length, not 'snx'
        if len(usdc) > 0:
            crypto_service.save_data_to_influx(usdc)