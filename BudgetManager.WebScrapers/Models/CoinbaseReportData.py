from dataclasses import dataclass
from datetime import datetime


@dataclass
class CoinbaseReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    total_unit: str
    operationType: str