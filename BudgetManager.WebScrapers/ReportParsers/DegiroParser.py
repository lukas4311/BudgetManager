import logging
import warnings
from csv import DictReader
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

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row: pd.Series) -> TradingReportData:
        """
        Map a single pandas Series row to TradingReportData model

        Args:
            row: pandas Series representing a single row from the DataFrame

        Returns:
            TradingReportData: Parsed trading data model
        """
        try:
            column_names = list(row.index)

            date = row["Datum"]
            ticker = row["ISIN"]
            name = row["Produkt"]
            number_of_shares = row["Počet"]
            total = float(row["Celkem"]) if pd.notna(row["Celkem"]) and row["Celkem"] != '' else 0.0
            isin = row["ISIN"]
            hodnota_index = column_names.index("Hodnota")
            share_currency_col = column_names[hodnota_index - 1]
            share_currency = row[share_currency_col]

            currency_col = column_names[hodnota_index + 1]
            currency = row[currency_col]

            pandas_date = pd.to_datetime(date)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date = pandas_date.tz_convert("utc")

            currency_id = self.__stockRepo.get_currency_id(currency)
            share_currency_id = self.__stockRepo.get_currency_id(share_currency)

            return TradingReportData(pandas_date, ticker, name, number_of_shares, total,
                                     currency_id, 'StockTradeTickers', isin, None, share_currency_id)

        except Exception as e:
            logging.error(f"Error mapping row to model: {str(e)}, Row index: {row.name}")
            return None

    def map_report_rows_to_model(self, df) -> list[TradingReportData]:
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
