import json
import urllib.request
import csv
import io
import pandas as pd
import requests
from typing import List
from Models.Fmp.StockPriceData import StockPriceData
import yfinance as yf
from datetime import datetime
from dataclasses import dataclass


@dataclass
class StockSplitData:
    """
    Data class representing stock split information.

    Attributes:
        date (datetime): Date when the stock split occurred
        split (str): Split ratio as string (e.g., "2:1", "3:2")
        split_coefficient (float): Numerical coefficient of the split
    """
    date: datetime
    split: str
    split_coefficient: float


@dataclass
class StockData:
    """
    Data class representing comprehensive stock information.

    Attributes:
        symbol (str): Stock ticker symbol (e.g., 'AAPL', 'GOOGL')
        currency (str): Trading currency (e.g., 'USD', 'EUR')
        exchange_name (str): Name of the exchange (e.g., 'NASDAQ', 'NYSE')
        prices (List[StockPriceData]): List of historical price data points
    """
    symbol: str
    currency: str
    exchange_name: str
    prices: List[StockPriceData]

    @classmethod
    def from_json(self, json_data: str) -> 'StockData':
        """
        Parse JSON string from Yahoo Finance API into StockData object.

        Converts timestamps and price data from Yahoo Finance's nested JSON
        structure into a structured StockData object with timezone handling.

        Args:
            json_data (str): Raw JSON response from Yahoo Finance API

        Returns:
            StockData: Parsed stock data with price history

        Raises:
            json.JSONDecodeError: If JSON data is malformed
            KeyError: If expected fields are missing from JSON
            ValueError: If timestamp conversion fails

        Example:
            >>> json_response = '{"chart": {...}}'
            >>> stock_data = StockData.from_json(json_response)
            >>> print(stock_data.symbol)
            'AAPL'
        """
        data = json.loads(json_data)

        # Extract data from nested structure
        chart_data = data['chart']['result'][0]
        meta = chart_data['meta']
        timestamps = chart_data['timestamp']
        quotes = chart_data['indicators']['quote'][0]

        # Create list of StockPrice objects with timezone conversion
        prices = [
            StockPriceData(
                date=pd.to_datetime(datetime.fromtimestamp(ts)).tz_localize("Europe/Prague").tz_convert("utc"),
                value=close
            )
            for ts, close in zip(timestamps, quotes['close'])
        ]

        return self(
            symbol=meta['symbol'],
            currency=meta['currency'],
            exchange_name=meta['exchangeName'],
            prices=prices
        )


class YahooService:
    """
    Service class for interacting with Yahoo Finance APIs.

    Provides methods to retrieve historical stock prices, split data,
    and company information using various Yahoo Finance endpoints.

    Attributes:
        timeSeriesKey (str): Constant for time series data key
        closeValueKey (str): Constant for closing price key
        token (str): API token (currently unused but reserved for future use)
    """

    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def get_stock_price_history(self, ticker: str, unix_from: str, unix_to: str) -> List[StockPriceData]:
        """
        Retrieve historical stock price data using CSV download endpoint.

        Downloads daily stock prices for the specified period and converts
        them to StockPriceData objects with UTC timezone.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL')
            unix_from (str): Start date as Unix timestamp string
            unix_to (str): End date as Unix timestamp string

        Returns:
            List[StockPriceData]: List of daily price data points
        """
        print('Downloading stock history for: ' + ticker)
        stock_price_data = []

        url = f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={unix_from}&period2={unix_to}&interval=1d&events=history&includeAdjustedClose=true'

        with urllib.request.urlopen(url) as response:
            data = response.read().decode()
            rows = csv.DictReader(io.StringIO(data))

            for row in rows:
                close = row["Close"]
                date = row["Date"]
                pandas_date = pd.to_datetime(date)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert("utc")

                try:
                    close_price = float(close)
                except ValueError:
                    continue

                price_model = StockPriceData(pandas_date, close_price)
                stock_price_data.append(price_model)

        return stock_price_data

    def get_stock_price_history_new(self, ticker: str, unix_from: str, unix_to: str) -> List[StockPriceData]:
        """
        Retrieve historical stock price data using JSON chart endpoint.

        Modern implementation using Yahoo Finance's chart API that returns
        JSON data instead of CSV. Includes better error handling and
        additional metadata.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL')
            unix_from (str): Start date as Unix timestamp string
            unix_to (str): End date as Unix timestamp string

        Returns:
            List[StockPriceData]: List of daily price data points
        """
        print('Downloading stock history for: ' + ticker)

        url = f'https://query1.finance.yahoo.com/v8/finance/chart/{ticker}?events=capitalGain%7Cdiv%7Csplit&formatted=true&includeAdjustedClose=true&interval=1d&period1={unix_from}&period2={unix_to}'
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        }

        try:
            response = requests.get(url, headers=headers)
            response.raise_for_status()
            stock_data = StockData.from_json(response.text)
            return stock_data.prices
        except requests.RequestException as e:
            raise requests.RequestException(f"Failed to fetch data: {str(e)}")
        except (KeyError, json.JSONDecodeError) as e:
            raise ValueError(f"Invalid response data: {str(e)}")

    def get_stock_split_history(self, ticker: str, unix_from: str, unix_to: str) -> list[StockSplitData]:
        """
        Retrieve historical stock split data for the specified period.

        Downloads stock split events and calculates split coefficients
        for the given ticker and time range.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL')
            unix_from (str): Start date as Unix timestamp string
            unix_to (str): End date as Unix timestamp string

        Returns:
            List[StockSplitData]: List of stock split events with coefficients
        """
        print('Downloading stock split history for: ' + ticker)
        stock_split_data = []

        try:
            url_definition = f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={unix_from}&period2={unix_to}&interval=1d&events=split&includeAdjustedClose=true'

            with urllib.request.urlopen(url_definition) as url:
                data = url.read().decode()
                rows = csv.DictReader(io.StringIO(data))

                for row in rows:
                    split = row["Stock Splits"]
                    date = row["Date"]

                    # Convert date to pandas datetime with timezone handling
                    pandas_date = pd.to_datetime(date)
                    pandas_date = pandas_date.tz_localize("Europe/Prague")
                    pandas_date = pandas_date.tz_convert("utc")

                    price_model = StockSplitData(
                        pandas_date,
                        split,
                        self.calculate_split_coefficient(split)
                    )
                    stock_split_data.append(price_model)
        except Exception:
            return stock_split_data

        return stock_split_data

    def get_company_name(self, ticker: str) -> str:
        """
        Retrieve the short company name for a given ticker symbol.

        Uses the yfinance library to fetch company information
        and extract the short name field.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL')

        Returns:
            str: Short company name, or None if not found
        """
        try:
            data = yf.Ticker(ticker)
            info = data.info
            return info["shortName"]
        except Exception as ex:
            print(ex)
            return None

    def calculate_split_coefficient(self, split_string: str) -> float:
        """
        Calculate the numerical coefficient from a stock split ratio string.

        Converts split ratios like "2:1" or "3:2" into their decimal
        equivalents (2.0, 1.5 respectively).

        Args:
            split_string (str): Split ratio in format "numerator:denominator"

        Returns:
            float: Calculated split coefficient
        """
        numbers = split_string.split(":")
        numerator = int(numbers[0])
        denominator = int(numbers[1])
        ratio = 1.0

        if denominator != 0:
            ratio = numerator / denominator

        return ratio