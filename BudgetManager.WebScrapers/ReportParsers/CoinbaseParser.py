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

    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
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