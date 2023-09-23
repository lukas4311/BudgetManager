import urllib.request
import csv
import io
import pandas as pd
from Models.Fmp.StockPriceData import StockPriceData
import yfinance as yf


class YahooService:
    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def get_stock_price_history(self, ticker: str, unix_from: str, unix_to: str):
        print('Downloading stock history for: ' + ticker)
        stockPriceData = []
        # with urllib.request.urlopen(f'https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1=511056000&period2=1673156073&interval=1d&events=history&includeAdjustedClose=true') as url:
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

    def get_company_name(self, ticker:str):
        data = yf.Ticker("AAPL")
        info = data.info
        return info["shortName"]


# yahooService = YahooService()
# yahooService.get_stock_price_history("MMM")