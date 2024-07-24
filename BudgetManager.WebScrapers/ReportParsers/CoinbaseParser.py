import base64 as b64
import csv
import io
from dataclasses import dataclass
from datetime import datetime
import pandas as pd
from Exceptions.ParseCsvError import ParseCsvError
from Models import TradingReportData
from ReportParsers.BrokerReportParser import BrokerReportParser
from Services.CryptoSqlService import CryptoSqlService
from Services.DB import StockRepository


@dataclass
class CoinbaseReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    total_unit: str
    operationType: str


class CoinbaseParser(BrokerReportParser):
    __stockRepo: StockRepository

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData:
        ticker = row['size unit']
        time = row['created at']
        buy_or_sell = row['side']
        size = float(row['size'])
        total = abs(float(row['total']))
        total_unit = row['price/fee/total unit']

        if buy_or_sell == "BUY":
            total = total * -1

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")
        currency_id = self.__stockRepo.get_currency_id(total_unit)

        return TradingReportData(pandas_date, ticker, ticker, size, total, currency_id)

    def map_report_rows_to_model(self, rows) -> list(TradingReportData):
        records = []

        for row in rows:
            stock_record = self.map_report_row_to_model(row)

            if stock_record is not None:
                records.append(stock_record)

        return records

    def map_csv_row_to_model(self, row):
        time = row['created at']
        buy_or_sell = row['side']
        size = float(row['size'])
        total = abs(float(row['total']))
        total_unit = row['price/fee/total unit']
        size_unit = row['size unit']

        if buy_or_sell == "BUY":
            total = total * -1

        pandas_date = pd.to_datetime(time)
        pandas_date = pandas_date.tz_convert("utc")

        return CoinbaseReportData(pandas_date, size_unit, size, total, total_unit, buy_or_sell)

    def load_coinbase_report_csv(self):
        with open("..\\BrokerReports\\Coinbase.csv", 'r') as file:
            rows = csv.DictReader(file)
            records = []
            for row in rows:
                coinbase_record = self.map_csv_row_to_model(row)
                records.append(coinbase_record)

        return records


def process_report_data(crypto_sql_service: CryptoSqlService, parser: CoinbaseParser):
    broker_report_data = crypto_sql_service.get_all_crypto_broker_reports_to_process()
    all_reports_data = []

    for report_data in broker_report_data:
        try:
            parse_report_data_to_model(all_reports_data, parser, report_data)
        except Exception as e:
            crypto_sql_service.changeProcessState(report_data.id, "ParsingError")

    print(all_reports_data)
    for parsed_report in all_reports_data:
        try:
            crypto_sql_service.store_trade_data(parsed_report["data"])
            crypto_sql_service.changeProcessState(parsed_report["report_id"], "Finished")
        except Exception as e:
            print(parsed_report)
            crypto_sql_service.changeProcessState(parsed_report["report_id"], "SavinggError")

def parse_report_data_to_model(all_reports_data, parser, report_data):
    try:
        parsed_csv = b64.b64decode(report_data.fileContentBase64).decode('utf-8')
        rows = csv.DictReader(io.StringIO(parsed_csv))
        records = []
        for row in rows:
            coinbase_record = parser.map_csv_row_to_model(row)
            records.append(coinbase_record)

        all_reports_data.append({"user_id": report_data.userIdentityId, "report_id": report_data.id, "data": records})
    except Exception as e:
        raise ParseCsvError("Error while parsing CSV")


# parser = CoinbaseParser()
# cryptoSqlService = CryptoSqlService()
# process_report_data(cryptoSqlService, parser)
