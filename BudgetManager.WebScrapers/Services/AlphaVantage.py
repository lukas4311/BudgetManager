import requests
import pandas as pd
from Models.Fmp.StockPriceData import StockPriceData
from config import alphavantageUrl


class AlphaVantageService:
    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def __init__(self, api_token: str):
        self.token = api_token

    def get_stock_price_history(self, ticker: str):
        print('Downloading ' + ticker)
        url = f'{alphavantageUrl}/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={ticker}&apikey={self.token}&outputsize=full'
        r = requests.get(url)
        data = r.json()
        time_series_data = data[self.timeSeriesKey]
        stock_price_data = []

        for key in time_series_data.keys():
            pandas_date = pd.to_datetime(key)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date = pandas_date.tz_convert("utc")
            price_model = StockPriceData(pandas_date, float(time_series_data[key][self.closeValueKey]))
            stock_price_data.append(price_model)

        return stock_price_data
