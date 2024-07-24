import csv
import pandas as pd
import logging
from datetime import datetime
from dataclasses import dataclass

from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session

import secret
from Models.TradingReportData import TradingReportData
from Orm.CurrencySymbol import CurrencySymbol
from Orm.StockTicker import StockTicker
from Orm.StockTradeHistory import Base, StockTradeHistory
from ReportParsers.BrokerReportParser import BrokerReportParser
from Services.DB.StockRepository import StockRepository
from Services.YahooService import YahooService

log_name = 'Logs/IB.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


@dataclass
class IBReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    currency: str
    name: str


class InteractiveBrokersParse(BrokerReportParser):
    __stockRepo: StockRepository

    def __init__(self):
        self.__stockRepo = StockRepository()

    def map_report_row_to_model(self, row) -> TradingReportData | None:
        if self.__check_row(row):
            return None

        currency = row[None][0]
        ticker = row[None][1]
        date = row[None][2]
        date = date.split(',')[0]
        number_of_shares = float(row[None][3])
        total_without_fee = float(row[None][6])
        total = float(row[None][8])
        buy = total_without_fee < 0
        total_with_action = total * (-1 if buy else 1)

        pandas_date = pd.to_datetime(date)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")
        currency_id = self.__stockRepo.get_currency_id(currency)

        return TradingReportData(pandas_date, ticker, ticker, number_of_shares, total_with_action, currency_id)

    def map_report_rows_to_model(self, rows) -> list[TradingReportData]:
        records = []

        for row in rows:
            stock_record = self.map_report_row_to_model(row)

            if stock_record is not None:
                records.append(stock_record)

        return records

    def __check_row(self, row):
        return row['Statement'] == 'Trades' and row['Field Value'] == 'Stocks' and row['Header'] == 'Data'
