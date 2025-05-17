from dataclasses import dataclass
from typing import List
from dacite import from_dict
from finnhub import Client

from secret import finnhubApiToken


@dataclass
class SymbolInfo:
    description: str
    displaySymbol: str
    symbol: str
    type: str


@dataclass
class StockResponse:
    count: int
    result: List[SymbolInfo]


@dataclass
class Stock:
    currency: str
    description: str
    displaySymbol: str
    figi: str
    mic: str
    symbol: str
    type: str


class FinnhubService:
    """
    A service class for interacting with the Finnhub API to retrieve stock symbol information.

    Attributes:
        __finnhub_client (Client): An instance of a client used to make API calls to Finnhub.
    """

    def __init__(self, finnhub_client: Client):
        """
        Initialize the FinnhubService with a given Finnhub client.

        Args:
            finnhub_client (Client): The API client for interacting with the Finnhub service.
        """
        self.__finnhub_client = finnhub_client

    def get_symbol_info_from_isin(self, isin: str) -> SymbolInfo | None:
        """
        Retrieve stock symbol information using an ISIN (International Securities Identification Number).

        Args:
            isin (str): The ISIN to look up.

        Returns:
            SymbolInfo | None: The symbol information if found, otherwise None.
        """
        response = self.__finnhub_client.symbol_lookup(isin)
        finnhub_symbol_model: StockResponse = from_dict(data_class=StockResponse, data=response)

        if finnhub_symbol_model.count == 0:
            return None

        return finnhub_symbol_model.result[0]

    def get_symbols(self, exchange_code: str, mic: str = None) -> list[Stock]:
        """
        Retrieve a list of stock symbols for a specific exchange.

        Args:
            exchange_code (str): The code of the exchange (e.g., 'US').
            mic (str, optional): The Market Identifier Code. Defaults to None.

        Returns:
            list[Stock]: A list of Stock objects representing the available symbols.
        """
        response = self.__finnhub_client.stock_symbols(exchange_code, mic)
        stocks = [from_dict(data_class=Stock, data=item) for item in response]
        print(response)
        # Print the parsed data
        for stock in stocks:
            print(f"Symbol: {stock.symbol}, Description: {stock.description}, ISNI: {stock.figi}")

        return stocks


# finhub = FinnhubService(Client(finnhubApiToken))
# data = finhub.get_symbols('L', 'XLON')
# print(data)