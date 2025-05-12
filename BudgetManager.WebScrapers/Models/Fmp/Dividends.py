from dataclasses import dataclass
from typing import List
from Models.Fmp.HistoricalDividend import HistoricalDividend


@dataclass
class Dividends:
    symbol: str
    historical: List[HistoricalDividend]

    def __init__(self, **data):
        self.symbol = data["symbol"]
        self.historical = []

        for value in data["historical"]:
            history_data = HistoricalDividend(**value)
            self.historical.append(history_data)

    @staticmethod
    def create_from_json(data):
        return Dividends(**data)
