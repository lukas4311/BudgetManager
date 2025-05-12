import logging
import warnings
from datetime import datetime
import pandas as pd
from Models.TradingReportData import TradingReportData
from ReportParsers.BrokerReportParser import BrokerReportParser
from Services.DB.StockRepository import StockRepository

warnings.filterwarnings('ignore', category=UserWarning)

log_name = 'Logs/degiro.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class DegiroReportParser(BrokerReportParser):
    __stockRepo: StockRepository
    __index_of_currency: int = -1

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData:
        date = row["Datum"]
        ticker = row["ISIN"]
        name = row["Produkt"]
        number_of_shares = row["Počet"]
        total = float(row["Celkem"])
        currency = list(row.values())[self.__index_of_currency] if self.__index_of_currency != -1 else 'EUR'
        isin = row["ISIN"]

        pandas_date = pd.to_datetime(date)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")
        currency_id = self.__stockRepo.get_currency_id(currency)

        return TradingReportData(pandas_date, ticker, name, number_of_shares, total, currency_id, 'StockTradeTickers', isin, None)

    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
        records = []
        fieldnames = rows.fieldnames
        self.__index_of_currency = fieldnames.index("Celkem") + 1

        for row in rows:
            stock_record = self.map_report_row_to_model(row)

            if stock_record is not None:
                records.append(stock_record)

        return records

