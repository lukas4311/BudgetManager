import csv
import time
from dataclasses import dataclass
import datetime
from datetime import timedelta
import pytz
from Scrapers.MacroTrendsScraper import MacroTrendScraper
from config import token, influxUrl, organizaiton
from Services.InfluxRepository import InfluxRepository
utc = pytz.UTC


@dataclass
class TickerRecord:
    ticker: str
    time: datetime


tickers = []
sp500 = []


def add_ticker_from_csv_file(rows, destination: list):
    for row in rows:
        symbol = row["Symbol"]
        if "^" not in symbol:
            destination.append(symbol)


def get_tickers_for_measurement(measurement: str):
    influx_repository = InfluxRepository(influxUrl, "Stocks", token, organizaiton)
    data = influx_repository.find_all_last_value_for_tag(measurement, "ticker")
    stored_tickers = []

    for table in data:
        for record in table.records:
            ticker = TickerRecord(record["ticker"], record["_time"])
            stored_tickers.append(ticker)

    return stored_tickers


with open("..\\SourceFiles\\nasdaq_screener_1649418624867.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    add_ticker_from_csv_file(csv_file, tickers)

with open("..\\SourceFiles\\nasdaq_screener_1649418684428.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    add_ticker_from_csv_file(csv_file, tickers)

with open("..\\SourceFiles\\nasdaq_screener_1649418699710.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    add_ticker_from_csv_file(csv_file, tickers)

with open("..\\SourceFiles\\sp500.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    add_ticker_from_csv_file(csv_file, sp500)

stored_tickers = get_tickers_for_measurement("CashFlow")
macro_trend = MacroTrendScraper()
# tickers = [tick for tick in sp500 if tick == "AAPL"]

# print(storedTickers)
# print(tickers)
# exit(9)

# INCOME STATEMENT
# for ticker in tickers:
#     skip_sleep = False
#     print("Searching for ticker: " + ticker)
#     founded: TickerRecord = [tickerInfo for tickerInfo in storedTickers if tickerInfo.ticker == ticker]
#
#     if founded:
#         print("Ticker was founded in Influx.")
#         if founded[0].time < utc.localize(datetime.datetime.utcnow() - timedelta(days=360)):
#             print(f'Company data are old. New data will be downloaded. Last data are form: {founded[0].time}' )
#             edgeTime = founded[0].time
#             macroTrend.download_income_statement_from_date(ticker, edgeTime)
#         else:
#             print("Company data are actual.")
#             skip_sleep = True
#     else:
#         print("Ticker data was not found in Influx. All company data will be stored.")
#         macroTrend.download_income_statement(ticker)
#
#     if not skip_sleep:
#         print("Waiting before new data:", end="")
#         for i in range(1, 6):
#             print(f'{i} ', end="")
#             time.sleep(1)
#
#         print('\n')

#  BALANCE SHEET
# for ticker in sp500:
#     try:
#         skip_sleep = False
#         print("Searching for ticker: " + ticker)
#         founded: TickerRecord = [tickerInfo for tickerInfo in storedTickers if tickerInfo.ticker == ticker]
#
#         if founded:
#             print("Ticker was founded in Influx.")
#             if founded[0].time < utc.localize(datetime.datetime.utcnow() - timedelta(days=360)):
#                 print(f'Company data are old. New data will be downloaded. Last data are form: {founded[0].time}' )
#                 edgeTime = founded[0].time
#                 macroTrend.download_balance_sheet_from_date(ticker, edgeTime)
#             else:
#                 print("Company data are actual.")
#                 skip_sleep = True
#         else:
#             print("Ticker data was not found in Influx. All company data will be stored.")
#             macroTrend.download_balance_sheet(ticker)
#
#         if not skip_sleep:
#             print("Waiting before new data:", end="")
#             for i in range(1, 6):
#                 print(f'{i} ', end="")
#                 time.sleep(1)
#
#             print('\n')
#     except Exception:
#         print(f'{ticker} cannot be downloaded')
#

#
for ticker in sp500:
    try:
        skip_sleep = False
        print("Searching for ticker: " + ticker)
        founded: TickerRecord = [tickerInfo for tickerInfo in stored_tickers if tickerInfo.ticker == ticker]

        if founded:
            print("Ticker was founded in Influx.")
            if founded[0].time < utc.localize(datetime.datetime.utcnow() - timedelta(days=360)):
                print(f'Company data are old. New data will be downloaded. Last data are form: {founded[0].time}')
                edgeTime = founded[0].time
                macro_trend.download_cash_flow_from_date(ticker, edgeTime)
            else:
                print("Company data are actual.")
                skip_sleep = True
        else:
            print("Ticker data was not found in Influx. All company data will be stored.")
            macro_trend.download_cash_flow(ticker)

        if not skip_sleep:
            print("Waiting before new data:", end="")
            for i in range(1, 6):
                print(f'{i} ', end="")
                time.sleep(1)

            print('\n')
    except Exception:
        print(f'{ticker} cannot be downloaded')
