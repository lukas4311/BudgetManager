from abc import abstractmethod, ABC
from dataclasses import dataclass
from datetime import datetime

from Models import TradingReportData


# @dataclass
# class TradingReportData:
#     time: datetime
#     ticker: str
#     name: str
#     number_of_shares: float
#     total: float
#
#
# @dataclass
# class CoinbaseReportData:
#     time: datetime
#     ticker: str
#     size: float
#     total: float
#     total_unit: str
#     operationType: str
#
#
# @dataclass
# class TradingReportData:
#     time: datetime
#     ticker: str
#     name: str
#     number_of_shares: float
#     total: float
#
#
# @dataclass
# class TradingReportData:
#     time: datetime
#     ticker: str
#     name: str
#     number_of_shares: float
#     total: float
#
#
# @dataclass
# class IBReportData:
#     time: datetime
#     ticker: str
#     size: float
#     total: float
#     currency: str
#     name: str


class BrokerReportParser(ABC):
    @abstractmethod
    def map_report_row_to_model(self, row) -> TradingReportData:
        pass
