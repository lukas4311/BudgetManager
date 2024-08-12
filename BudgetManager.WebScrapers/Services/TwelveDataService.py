from typing import List

from twelvedata import TDClient


class TwelveDataService:
    logging: object
    apiToken: str

    def __init__(self, api_token: str, logging: object):
        self.logging = logging
        self.token = api_token
        self.__twelve_data_client = TDClient(apikey=api_token)

    def get_ticker_info(self, ticker: str):
        self.__twelve_data_client.symbol_search(symbol=ticker)

    def get_tickers_info(self, tickers: List[str]):
        symbols = ','.join(tickers)
        self.__twelve_data_client.symbol_search(symbol=symbols)
