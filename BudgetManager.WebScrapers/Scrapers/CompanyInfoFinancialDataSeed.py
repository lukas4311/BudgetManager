import csv
from dataclasses import dataclass
import datetime
from datetime import timedelta
import pytz
from influxdb_client import Point, WritePrecision

from Scrapers.MacroTrendsScraper import MacroTrendScraper
from secret import influxDbUrl
from configManager import token
from configManager import organizaiton
from Services.InfluxRepository import InfluxRepository
utc=pytz.UTC

@dataclass
class TickerRecord:
    ticker: str
    time: datetime


tickers = []


def addTickerFromCsvFile(rows):
    for row in rows:
        symbol = row["Symbol"]
        if "^" not in symbol:
            tickers.append(symbol)


with open("..\\SourceFiles\\nasdaq_screener_1649418624867.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    addTickerFromCsvFile(csv_file)

with open("..\\SourceFiles\\nasdaq_screener_1649418684428.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    addTickerFromCsvFile(csv_file)

with open("..\\SourceFiles\\nasdaq_screener_1649418699710.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    addTickerFromCsvFile(csv_file)


influx_repository = InfluxRepository(influxDbUrl, "Stocks", token, organizaiton)
data = influx_repository.find_all_last_value_for_tag("IncomeStatement", "ticker")
storedTickers = []

for table in data:
    for record in table.records:
        ticker = TickerRecord(record["ticker"], record["_time"])
        storedTickers.append(ticker)

print(tickers)
macroTrend = MacroTrendScraper()

for ticker in tickers:
    founded: TickerRecord = [tickerInfo for tickerInfo in storedTickers if tickerInfo.ticker == ticker]

    if founded:
        print(founded[0].time)
        if founded[0].time < utc.localize(datetime.datetime.utcnow() - timedelta(days=90)):
            edgeTime = founded[0].time
            # TODO download new data
    else:
        macroTrend.download_income_statement(ticker)


# tickerList = [x.ticker for x in storedTickers]
# print(tickerList)
