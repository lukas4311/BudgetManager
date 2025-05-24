import logging
import warnings
from datetime import datetime
import pandas as pd
from Models.TradingReportData import TradingReportData
from ReportParsers.BrokerReportParser import BrokerReportParser
from Services.DB.StockRepository import StockRepository

warnings.filterwarnings('ignore', category=UserWarning)

log_name = 'Logs/trading212.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class Trading212ReportParser(BrokerReportParser):
    __stockRepo: StockRepository
    __total_index: int = -1
    __total_index_currency: str = ""

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData:
        action = row["Action"]
        time = row["Time"]
        ticker = row["Ticker"]
        name = row["Name"]
        number_of_shares = float(row["No. of shares"])
        total = float(row[self.__total_index])
        currency_total = self.__total_index_currency
        isin = row["ISIN"]
        transaction_id = row["ID"]
        share_currency = row["Currency (Price / share)"]

        if action == "Market buy":
            total = total * -1

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")

        currency_id = self.__stockRepo.get_currency_id(currency_total)
        share_currency_id = self.__stockRepo.get_currency_id(share_currency)

        return TradingReportData(pandas_date, ticker, name, number_of_shares, total, currency_id, 'StockTradeTickers', isin, transaction_id, share_currency_id)

    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
        records = []

        for i, row in enumerate(rows):
            if i == 0:
                total_indexes = self.__find_all_indices_containing(rows.fieldnames, "Total")
                total_index = total_indexes[0]
                self.__total_index = rows.fieldnames[total_index]
                self.__total_index_currency = self.__extract_currency_from_header(self.__total_index)

            stock_record = self.map_report_row_to_model(row)
            records.append(stock_record)

        return records

    def __find_all_indices_containing(self, arr, literal):
        return [i for i, string in enumerate(arr) if literal in string]

    def __extract_currency_from_header(self, header):
        start = header.find('(')
        end = header.find(')')

        if start != -1 and end != -1 and end > start:
            return header[start + 1:end]
        return None