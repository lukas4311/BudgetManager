import logging
import warnings
from datetime import datetime
import pandas as pd
from pandas import DataFrame

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

    def map_report_row_to_model(self, row: pd.Series) -> TradingReportData:
        action = row["Action"]
        time = row["Time"]
        ticker = row["Ticker"]
        name = row["Name"]
        number_of_shares = float(row["No. of shares"])
        total = float(row["Total"])
        currency_total = row["Currency (Total)"]
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

        return TradingReportData(pandas_date, ticker, name, number_of_shares, total, currency_id, 'StockTradeTickers',
                                 isin, transaction_id, share_currency_id)

    def map_report_rows_to_model(self, df: DataFrame) -> list[TradingReportData]:
        records = []
        total_rows = len(df)

        logging.info(f"Processing {total_rows} rows")

        for index, row in df.iterrows():
            try:
                stock_record = self.map_report_row_to_model(row)
                if stock_record is not None:
                    records.append(stock_record)
            except Exception as e:
                logging.error(f"Error processing row {index}: {str(e)}")
                continue

        logging.info(f"Successfully processed {len(records)} out of {total_rows} rows")
        return records
