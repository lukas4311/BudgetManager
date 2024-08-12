import json
import logging
from dataclasses import dataclass
from datetime import datetime
from typing import List

from dacite import from_dict
from twelvedata import TDClient

from secret import tokenTwelveData

log_name = 'Logs/Twelvedata.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


@dataclass
class SymbolInfo:
    symbol: str
    instrument_name: str
    exchange: str
    mic_code: str
    exchange_timezone: str
    instrument_type: str
    country: str
    currency: str

@dataclass
class SymbolResponse:
    symbols: List[SymbolInfo]

class TwelveDataService:
    logging: object
    apiToken: str

    def __init__(self, api_token: str, logging: object):
        self.logging = logging
        self.token = api_token
        self.__twelve_data_client = TDClient(apikey=api_token)

    def get_ticker_info(self, ticker: str) -> SymbolInfo:
        json_data = self.__twelve_data_client.symbol_search(symbol=ticker).as_json();
        response = SymbolResponse(symbols=[from_dict(data_class=SymbolInfo, data=item) for item in json_data])
        return response.symbols[0]

    def get_tickers_info(self, tickers: List[str]) -> SymbolInfo:
        symbols = ','.join(tickers)
        return self.__twelve_data_client.symbol_search(symbol=symbols).as_json()

twelvedata = TwelveDataService(tokenTwelveData, logging)
data = twelvedata.get_ticker_info('AAPL')
print(data)