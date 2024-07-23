import csv
import logging
import warnings
from datetime import datetime
import pandas as pd
from Models.TradingReportData import TradingReportData

warnings.filterwarnings('ignore', category=UserWarning)

log_name = 'Logs/degiro.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class Trading212ReportParser:
    def read_report_csv_file(self):
        parsed_data: list[TradingReportData] = []
        with open("..\\BrokerReports\\Degiro.csv", 'r') as file:
            rows = csv.DictReader(file)
            for row in rows:
                action = row["Datum"]
                time = row["Čas"]
                ticker = row["ISIN"]
                name = row["Produkt"]
                number_of_shares = row["Počet"]
                total = float(row["Celkem"])

                if action == "Market buy":
                    total = total * -1

                pandas_date = pd.to_datetime(time)
                pandas_date = pandas_date.tz_localize("Europe/Prague")
                pandas_date = pandas_date.tz_convert("utc")

                try:
                    model = TradingReportData(pandas_date, ticker, name, number_of_shares, total)
                    parsed_data.append(model)
                except Exception as e:
                    print(ticker + " - error - " + e)
        return parsed_data