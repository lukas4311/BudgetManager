from abc import abstractmethod, ABC
from csv import DictReader, reader

import pandas as pd
from pandas import DataFrame

from Models import TradingReportData


class BrokerReportParser(ABC):
    @abstractmethod
    def map_report_row_to_model(self, row: pd.Series) -> TradingReportData:
        pass

    @abstractmethod
    def map_report_rows_to_model(self, rows: DataFrame) -> list[TradingReportData]:
        pass
