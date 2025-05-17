import requests
import pandas as pd
from Models.Fmp.StockPriceData import StockPriceData
from config import alphavantageUrl


class AlphaVantageService:
    # Keys used to extract relevant data from the Alpha Vantage API response
    timeSeriesKey = "Time Series (Daily)"
    closeValueKey = "4. close"
    token: str

    def __init__(self, api_token: str):
        """
        Initializes the AlphaVantageService with the given API token.

        Args:
            api_token (str): API key for authenticating with Alpha Vantage.
        """
        self.token = api_token

    def get_stock_price_history(self, ticker: str) -> list[StockPriceData]:
        """
        Retrieves the historical daily stock prices for a given ticker.

        Args:
            ticker (str): The stock ticker symbol (e.g., 'AAPL').

        Returns:
            list[StockPriceData]: List of StockPriceData objects with date and close price.
        """
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
            price_model = StockPriceData(
                pandas_date,
                float(time_series_data[key][self.closeValueKey])
            )
            stock_price_data.append(price_model)

        return stock_price_data
