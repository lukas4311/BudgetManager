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

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData:
        action = row["Action"]
        time = row["Time"]
        ticker = row["Ticker"]
        name = row["Name"]
        number_of_shares = float(row["No. of shares"])
        total = float(row["Total"])
        currency_total = row["Currency (Total)"]

        if action == "Market buy":
            total = total * -1

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")

        currency_id = self.__stockRepo.get_currency_id(currency_total)

        return TradingReportData(pandas_date, ticker, name, number_of_shares, total, currency_id, 'StockTradeTickers')

    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
        records = []
        for row in rows:
            stock_record = self.map_report_row_to_model(row)
            records.append(stock_record)

        return records