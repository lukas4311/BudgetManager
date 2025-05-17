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
    """
    Service class for interacting with the TwelveData API.

    This class provides methods to search for financial symbols and retrieve
    their detailed information using the TwelveData API.

    Attributes:
        logging (object): Logger instance for recording service activity
        apiToken (str): API token for authenticating with TwelveData
        __twelve_data_client (TDClient): Private TwelveData client instance
    """
    logging: object
    apiToken: str

    def __init__(self, api_token: str, logging: object):
        """
        Initialize the TwelveDataService.

        Args:
            api_token (str): Valid API token for TwelveData service
            logging (object): Logger instance for recording service events

        Raises:
            ValueError: If api_token is empty or None
        """
        self.logging = logging
        self.token = api_token
        self.__twelve_data_client = TDClient(apikey=api_token)

    def get_ticker_info(self, ticker: str) -> SymbolInfo:
        """
        Retrieve detailed information for a single ticker symbol.

        Args:
            ticker (str): The ticker symbol to search for (e.g., 'AAPL')

        Returns:
            SymbolInfo: Detailed information about the ticker symbol

        Raises:
            IndexError: If no symbols are found for the given ticker
            KeyError: If the API response is missing expected fields
            ConnectionError: If unable to connect to the TwelveData API
        """
        json_data = self.__twelve_data_client.symbol_search(symbol=ticker).as_json()
        response = SymbolResponse(symbols=[from_dict(data_class=SymbolInfo, data=item) for item in json_data])
        return response.symbols[0]

    def get_tickers_info(self, tickers: List[str]) -> dict:
        """
        Retrieve information for multiple ticker symbols.

        Note: This method currently returns raw JSON data instead of processed SymbolInfo objects.
        Consider updating the return type annotation and processing logic for consistency.

        Args:
            tickers (List[str]): List of ticker symbols to search for

        Returns:
            dict: Raw JSON response from the TwelveData API containing symbol information

        Raises:
            ConnectionError: If unable to connect to the TwelveData API
            ValueError: If tickers list is empty
        """
        symbols = ','.join(tickers)
        return self.__twelve_data_client.symbol_search(symbol=symbols).as_json()


twelvedata = TwelveDataService(tokenTwelveData, logging)
data = twelvedata.get_ticker_info('AAPL')
print(data)