from abc import abstractmethod, ABC
from csv import DictReader, reader

from Models import TradingReportData


class BrokerReportParser(ABC):
    @abstractmethod
    def map_report_row_to_model(self, row) -> TradingReportData:
        pass

    @abstractmethod
    def map_report_rows_to_model(self, rows: DictReader[str]) -> list[TradingReportData]:
        pass