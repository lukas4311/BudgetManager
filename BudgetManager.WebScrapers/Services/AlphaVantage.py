from datetime import datetime
import requests
from dataclasses import dataclass
import pandas as pd


@dataclass
class StockPriceData:
    date: datetime
    value: float


class AlphaVantageService:
    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def __init__(self, apiToken: str):
        self.token = apiToken

    def get_stock_price_history(self, ticker: str):
        print('Downloading ' + ticker)
        url = 'https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=IBM&apikey=demo'
        r = requests.get(url)
        data = r.json()
        timeSeriesData = data[self.timeSeriesKey]
        print(timeSeriesData)

        for key in timeSeriesData.keys():
            pandas_date = pd.to_datetime(key)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date = pandas_date.tz_convert("utc")
            priceModel = StockPriceData(pandas_date, float(timeSeriesData[key][self.closeValueKey]))
            print(priceModel)
