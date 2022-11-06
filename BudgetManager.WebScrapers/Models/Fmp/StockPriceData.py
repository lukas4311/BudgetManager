from datetime import datetime
from dataclasses import dataclass


@dataclass
class StockPriceData:
    date: datetime
    value: float
