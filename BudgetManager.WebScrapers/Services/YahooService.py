import json
import urllib.request
import csv
import io
import pandas as pd
import requests
from typing import List, Optional
from Models.Fmp.StockPriceData import StockPriceData
import yfinance as yf
from datetime import datetime
from dataclasses import dataclass


@dataclass
class StockSplitData:
    date: datetime
    split: str
    split_coefficient: float


@dataclass
class StockData:
    symbol: str
    currency: str
    exchange_name: str
    prices: List[StockPriceData]

    @classmethod
    def from_json(self, json_data: str) -> 'StockData':
        """Parse JSON string into StockData object"""
        data = json.loads(json_data)

        # Extract data from nested structure
        chart_data = data['chart']['result'][0]
        meta = chart_data['meta']
        timestamps = chart_data['timestamp']
        quotes = chart_data['indicators']['quote'][0]
        # Create list of StockPrice objects
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
    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def get_stock_price_history(self, ticker: str, unix_from: str, unix_to: str):
        print('Downloading stock history for: ' + ticker)
        stockPriceData = []
        with urllib.request.urlopen(f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={unix_from}&period2={unix_to}&interval=1d&events=history&includeAdjustedClose=true') as url:
            data = url.read().decode()
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

                priceModel = StockPriceData(pandas_date, close_price)
                stockPriceData.append(priceModel)

        return stockPriceData

    def get_stock_price_history_new(self, ticker: str, unix_from: str, unix_to: str):
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
        print('Downloading stock split history for: ' + ticker)
        stock_split_data = []

        try:
            urlDefinition = f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={unix_from}&period2={unix_to}&interval=1d&events=split&includeAdjustedClose=true'
            with urllib.request.urlopen(urlDefinition) as url:
                data = url.read().decode()
                rows = csv.DictReader(io.StringIO(data))
                for row in rows:

                    split = row["Stock Splits"]
                    date = row["Date"]
                    pandas_date = pd.to_datetime(date)
                    pandas_date = pandas_date.tz_localize("Europe/Prague")
                    pandas_date = pandas_date.tz_convert("utc")

                    priceModel = StockSplitData(pandas_date, split, self.calculate_split_coefficient(split))
                    stock_split_data.append(priceModel)
        except:
            return stock_split_data

        return stock_split_data

    def get_company_name(self, ticker: str):
        try:
            data = yf.Ticker(ticker)
            info = data.info
            # print(data)
            return info["shortName"]
        except Exception as ex:
            print(ex)
            return None

    def calculate_split_coefficient(self, split_string: str) -> float:
        numbers = split_string.split(":")
        numerator = int(numbers[0])
        denominator = int(numbers[1])
        ratio = 1.0

        if denominator != 0:
            ratio = numerator / denominator

        return ratio

# yahoo_service = YahooService()
# yahoo_service.get_company_name('AAPL')

url = 'https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=aapl&apikey=demo'
r = requests.get(url)
data = r.json()

print(data)