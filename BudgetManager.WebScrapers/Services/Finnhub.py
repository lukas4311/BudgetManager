from dataclasses import dataclass
from typing import List
from dacite import from_dict
from finnhub import Client


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


class FinnhubService:

    def __init__(self, finnhub_client: Client):
        self.__finnhub_client = finnhub_client

    def get_symbol_info_from_isin(self, isin: str) -> SymbolInfo | None:
        response = self.__finnhub_client.symbol_lookup(isin)
        finnhub_symbol_model: StockResponse = from_dict(data_class=StockResponse, data=response)

        if finnhub_symbol_model.count == 0:
            return None

        return finnhub_symbol_model.result[0]
