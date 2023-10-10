import urllib.request
import csv
import io
import pandas as pd
from Models.Fmp.StockPriceData import StockPriceData
import yfinance as yf
from datetime import datetime
from dataclasses import dataclass


@dataclass
class StockSplitData:
    date: datetime
    split: str
    split_coefficient: str


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
                close_price = 0
                try:
                    close_price = float(close)
                except ValueError:
                    continue

                priceModel = StockPriceData(pandas_date, close_price)
                stockPriceData.append(priceModel)

        return stockPriceData

    def get_stock_split_history(self, ticker: str, unix_from: str, unix_to: str):
        print('Downloading stock history for: ' + ticker)
        stock_split_data = []
        urlDefinition = f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={unix_from}&period2={unix_to}&interval=1d&events=split&includeAdjustedClose=true'
        print(urlDefinition)
        with urllib.request.urlopen(urlDefinition) as url:
            data = url.read().decode()
            print(data)
            rows = csv.DictReader(io.StringIO(data))
            print(rows)
            for row in rows:

                split = row["Stock Splits"]
                date = row["Date"]
                pandas_date = pd.to_datetime(date)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert("utc")
                print(split)

                priceModel = StockSplitData(pandas_date, split, self.calculate_split_coefficient(split))
                stock_split_data.append(priceModel)

        return stock_split_data

    def get_company_name(self, ticker:str):
        try:
            data = yf.Ticker(ticker)
            info = data.info
            return info["shortName"]
        except:
            return None

    def calculate_split_coefficient(self, split_string: str) -> float:
        numbers = split_string.split(":")
        numerator = int(numbers[0])
        denominator = int(numbers[1])
        ratio = 1.0

        if denominator != 0:
            ratio = numerator / denominator

        return ratio
