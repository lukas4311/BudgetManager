from dataclasses import dataclass
from datetime import datetime

@dataclass
class TradingReportData:
    time: datetime
    ticker: str
    name: str
    number_of_shares: float
    total: float
    currency_id: str
