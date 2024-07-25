from abc import abstractmethod, ABC
from dataclasses import dataclass
from datetime import datetime

from Models import TradingReportData


class BrokerReportParser(ABC):
    @abstractmethod
    def map_report_row_to_model(self, row) -> TradingReportData:
        pass

    @abstractmethod
    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
        pass