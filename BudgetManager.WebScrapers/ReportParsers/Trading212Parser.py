import base64 as b64
import csv
import io
import logging
import warnings
from datetime import datetime
import pandas as pd
from Exceptions.ParseCsvError import ParseCsvError
from Models.TradingReportData import TradingReportData
from Services.DB.StockRepository import StockRepository

warnings.filterwarnings('ignore', category=UserWarning)

log_name = 'Logs/trading212.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class Trading212ReportParser:
    def read_report_csv_file(self):
        parsed_data: list[TradingReportData] = []
        with open("..\\BrokerReports\\Trading212_1.csv", 'r') as file:
            rows = csv.DictReader(file)
            for row in rows:
                action = row["Action"]
                time = row["Time"]
                ticker = row["Ticker"]
                name = row["Name"]
                number_of_shares = row["No. of shares"]
                total = float(row["Total"])

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

    def map_csv_row_to_model(self, row):
        action = row["Action"]
        time = row["Time"]
        ticker = row["Ticker"]
        name = row["Name"]
        number_of_shares = float(row["No. of shares"])
        total = float(row["Total"])

        if action == "Market buy":
            total = total * -1

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")

        return TradingReportData(pandas_date, ticker, name, number_of_shares, total)

    def save_report_data_to_db(self):
        data = self.read_report_csv_file()

        for tradeData in data:
            try:
                print(tradeData)
                self.store_orders(tradeData)
            except Exception as e:
                logging.info(
                    'Error while saving for ticker: ' + tradeData.ticker + 'and for time' + tradeData.time.strftime(
                        '%Y-%m-%d'))
                logging.error(e)


def process_report_data(stock_repo: StockRepository, parser: Trading212ReportParser):
    broker_report_data = stock_repo._get_all_stock_broker_reports_to_process()
    all_reports_data = []

    for report_data in broker_report_data:
        try:
            parse_report_data_to_model(all_reports_data, parser, report_data)
        except Exception as e:
            stock_repo.changeProcessState(report_data.id, "ParsingError")

    print(all_reports_data)
    for parsed_report in all_reports_data:
        try:
            stock_repo.store_trade_data(parsed_report["data"], "CZK", parsed_report["user_id"])
            stock_repo.changeProcessState(parsed_report["report_id"], "Finished")
        except Exception as e:
            print(parsed_report)
            stock_repo.changeProcessState(parsed_report["report_id"], "SavingError")


def parse_report_data_to_model(all_reports_data, parser, report_data):
    try:
        parsed_csv = b64.b64decode(report_data.fileContentBase64).decode('utf-8')
        rows = csv.DictReader(io.StringIO(parsed_csv))
        records = []
        for row in rows:
            stock_record = parser.map_csv_row_to_model(row)
            records.append(stock_record)

        all_reports_data.append({"user_id": report_data.userIdentityId, "report_id": report_data.id, "data": records})
    except Exception as e:
        raise ParseCsvError("Error while parsing CSV")


parser = Trading212ReportParser()
stockRepo = StockRepository()
process_report_data(stockRepo, parser)
