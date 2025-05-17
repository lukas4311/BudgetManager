import json
import logging
from dataclasses import dataclass
from datetime import datetime, timezone

import pandas as pd
import requests
from influxdb_client import Point, WritePrecision

from Models.FilterTuple import FilterTuple
from Services.InfluxRepository import InfluxRepository
from secret import token, organizationId
from config import influxUrl
from secret import tokenTwelveData

log_name = 'Logs/forexPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxUrl, "ForexV2", token, organizationId, logging)
measurement = "ExchangeRates"


@dataclass
class PriceModel:
    """
    Represents a single forex exchange rate data point.

    Attributes:
        datetime: The timestamp of the exchange rate (pandas Timestamp with timezone)
        close_price: The closing exchange rate value
        symbol: The currency pair symbol (e.g., 'USD/EUR', 'EUR/USD')
    """
    datetime: pd.Timestamp
    close_price: float
    symbol: str


class TwelveDataUrlBuilder:
    """
    Utility class for building URLs for the Twelve Data API.

    Provides static methods to construct properly formatted API URLs
    with the necessary parameters for time series data retrieval.
    """

    @staticmethod
    def build_time_series_url(symbols):
        """
        Build a time series URL for the Twelve Data API.

        Args:
            symbols: List of currency pair symbols to fetch

        Returns:
            str: Complete API URL with all necessary parameters
        """
        base_url = "https://api.twelvedata.com/time_series"
        symbol_param = ','.join(symbols)
        interval = "1day"
        api_key = tokenTwelveData
        return f"{base_url}?symbol={symbol_param}&interval={interval}&apikey={api_key}&start_date=2000-01-01"


class ApiDataSource:
    """
    Generic API data source for fetching and parsing forex data.

    This class handles HTTP requests to the API and converts the JSON
    response into PriceModel objects for further processing.
    """

    def __init__(self, api_url):
        """
        Initialize the API data source.

        Args:
            api_url: The API endpoint URL (can be None and set later)
        """
        self.api_url = api_url

    def fetch_data(self):
        """
        Fetch data from the configured API URL.

        Returns:
            dict: Parsed JSON response from the API
        """
        response = requests.get(self.api_url)

        if response.status_code != 200:
            raise ValueError("Failed to fetch data from the API.")

        return response.json()

    def parse_data(self, data) -> PriceModel:
        """
        Parse the API response data into PriceModel objects.

        Converts the JSON response from Twelve Data API into a list of
        standardized PriceModel objects with proper timezone handling.

        Args:
            data: Raw JSON data from the API response

        Returns:
            list[PriceModel]: List of parsed price data points
        """
        parsed_data = []
        for symbol, symbol_data in data.items():
            if 'values' not in symbol_data:
                continue

            for entry in symbol_data['values']:
                datetime_str = entry['datetime']
                close_price = entry['close']
                datetime = pd.to_datetime(datetime_str, utc=True)
                model = PriceModel(datetime=datetime, close_price=round(float(close_price), 6), symbol=symbol)
                parsed_data.append(model)

        return parsed_data


class ForexRateAnalyzer:
    """
    Analyzer for calculating cross rates and inverse rates from base currency pairs.

    This class provides functionality to expand the available forex data by
    calculating additional currency pairs from the base pairs fetched from the API.
    """

    def find_all_cross_rates(self, price_data: dict) -> dict:
        """
        Calculate cross rates from existing currency pairs.

        For example, if you have USD/EUR and USD/GBP, this method will
        calculate EUR/GBP cross rates by dividing the rates appropriately.

        Args:
            price_data: Dictionary of currency pairs and their historical data

        Returns:
            dict: Dictionary containing calculated cross rates
        """
        cross_data = {}

        for first_symbol, first_symbol_exchange_rates in price_data.items():
            for second_symbol, second_symbol_exchange_rates in price_data.items():
                if first_symbol == second_symbol or second_symbol + "/" + first_symbol in price_data:
                    continue
                cross_key = first_symbol + "/" + second_symbol
                cross_value = list(zip(first_symbol_exchange_rates, second_symbol_exchange_rates))

                for v1, v2 in cross_value:
                    if v1.datetime == v2.datetime:
                        cross_rate = round(v1.close_price / v2.close_price, 6)
                        cross_model = PriceModel(v1.datetime, cross_rate, cross_key)
                        base_currency = cross_key.split("/")[1]
                        quote_currency = cross_key.split("/")[-1]
                        key = quote_currency + "/" + base_currency
                        cross_model.symbol = key
                        if key in cross_data:
                            cross_data[key].append(cross_model)
                        else:
                            cross_data[key] = [cross_model]

        return cross_data

    def get_reversed_data(self, price_data: dict):
        """
        Calculate inverse/reversed currency pairs from existing pairs.

        For example, if you have USD/EUR, this method will calculate EUR/USD
        by taking the reciprocal of each rate (1 / USD_EUR_rate).

        Args:
            price_data: Dictionary of currency pairs and their historical data

        Returns:
            dict: Dictionary containing inverse currency pairs
        """
        reverse_data = {}

        for symbol, entries in price_data.items():
            reversed_symbol = self.__reverse_symbol(symbol)
            reversed_entries = [PriceModel(entry.datetime, 1 / entry.close_price, reversed_symbol) for entry in entries]
            reverse_data[reversed_symbol] = reversed_entries

        return reverse_data

    def __reverse_symbol(self, symbol):
        """
        Reverse a currency pair symbol.

        Args:
            symbol: Currency pair in format 'XXX/YYY'

        Returns:
            str: Reversed currency pair in format 'YYY/XXX'
        """
        return f"{symbol.split('/')[1]}/{symbol.split('/')[0]}"


class ForexScrapeService:
    """
    Service class for coordinating forex data scraping operations.

    This class acts as a controller that coordinates between the data source
    and URL builder to fetch forex data from the API.
    """

    def __init__(self):
        """Initialize the scrape service with no data source set."""
        self.data_source = None

    def set_data_source(self, data_source):
        """
        Set the data source for fetching API data.

        Args:
            data_source: An instance of ApiDataSource or compatible class
        """
        self.data_source = data_source

    def get_data(self, symbols) -> list[PriceModel]:
        """
        Fetch forex data for the specified currency pairs.

        This method builds the API URL, fetches the data, and parses it
        into PriceModel objects ready for further processing.

        Args:
            symbols: List of currency pair symbols to fetch

        Returns:
            list[PriceModel]: List of parsed forex rate data
        """
        if not self.data_source:
            raise ValueError("Data source is not set. Please call set_data_source() first.")

        api_url = TwelveDataUrlBuilder.build_time_series_url(symbols)
        self.data_source.api_url = api_url
        json_data = self.data_source.fetch_data()

        return self.data_source.parse_data(json_data)


class ForexService:
    """
    Main service class for forex data collection and storage.
    """

    def run(self):
        """
        Execute the complete forex data collection process.
        """
        symbols = ["USD/CZK", "USD/EUR", "USD/GBP", "USD/CHF", "USD/JPY"]
        service = ForexScrapeService()
        api_data_source = ApiDataSource(None)
        service.set_data_source(api_data_source)
        data = service.get_data(symbols)

        symbol_models = {}
        for model in data:
            if model.symbol in symbol_models:
                symbol_models[model.symbol].append(model)
            else:
                symbol_models[model.symbol] = [model]

        forex_rate_analyzer = ForexRateAnalyzer()
        inverse_rates = forex_rate_analyzer.get_reversed_data(symbol_models)
        all_cross_rates = forex_rate_analyzer.find_all_cross_rates(symbol_models)

        symbol_models.update(inverse_rates)
        symbol_models.update(all_cross_rates)

        for key in symbol_models:
            print(f'{key}: [{symbol_models[key][-1].symbol}]')

            exchange_rates = symbol_models[key]
            last_record = self.get_last_record_time(key)
            print(f"last record: {last_record}")
            filtered_exchange_rates = [d for d in exchange_rates if
                                       datetime.now().astimezone(d.datetime.tzinfo) > d.datetime > last_record]

            if len(filtered_exchange_rates) > 0:
                self.save_data_to_influx(filtered_exchange_rates)
            else:
                print(f"No new data for pair: {key}")

    def save_data_to_influx(self, priceData: list[PriceModel]):
        """
        Save forex exchange rate data to InfluxDB.

        This method converts PriceModel objects to InfluxDB Points
        and writes them to the database in batch format.

        Args:
            priceData: List of PriceModel objects to save
        """
        points_to_save = []
        transferred_symbol = priceData[0].symbol.replace('/', '-')
        logging.info('Saving forex pair: ' + transferred_symbol)

        for price_model in priceData:
            point = Point(measurement) \
                .tag("pair", price_model.symbol.replace('/', '-')) \
                .field('price', price_model.close_price)
            point = point.time(price_model.datetime, WritePrecision.NS)
            points_to_save.append(point)

        influx_repository.add_range(points_to_save)
        for point in points_to_save:
            print(point.to_line_protocol())

        influx_repository.save()
        logging.info('Data saved for pair: ' + priceData[0].symbol)
        print("Data saved")

    def get_last_record_time(self, ticker: str):
        """
        Retrieve the timestamp of the last recorded exchange rate for a currency pair.

        This method queries InfluxDB to find the most recent data point for the
        specified currency pair, enabling incremental data updates.

        Args:
            ticker: Currency pair symbol (e.g., 'USD/EUR')

        Returns:
            datetime: Timestamp of the last recorded data point, or default date if none exists
        """
        transferred_symbol = ticker.replace('/', '-')
        last_value = influx_repository.filter_last_value(measurement, FilterTuple("pair", transferred_symbol),
                                                         datetime.min)
        last_downloaded_time = datetime(1975, 1, 1, 0, 0, 0, tzinfo=timezone.utc)

        if len(last_value) != 0:
            last_downloaded_time = last_value[0].records[0]["_time"]

        return last_downloaded_time