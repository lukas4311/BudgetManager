import csv
import time
from dataclasses import dataclass
import datetime
from datetime import timedelta
import pytz
from Scrapers.MacroTrendsScraper import MacroTrendScraper
from secret import influxDbUrl
from configManager import token
from configManager import organizaiton
from Services.InfluxRepository import InfluxRepository
utc = pytz.UTC

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
# tickers = [tick for tick in tickers if tick == "AMZN"]

for ticker in tickers:
    print("Searching for ticker: " + ticker)
    founded: TickerRecord = [tickerInfo for tickerInfo in storedTickers if tickerInfo.ticker == ticker]

    if founded:
        print("Ticker was founded in Influx.")
        if founded[0].time < utc.localize(datetime.datetime.utcnow() - timedelta(days=90)):
            print("Company data are old. New data will be downloaded.")
            edgeTime = founded[0].time
            # macroTrend.download_income_statement(ticker, edgeTime)
        else:
            print("Company data are actual.")
    else:
        print("Ticker data was not found in Influx. All company data will be stored.")
        # macroTrend.download_income_statement(ticker)

    time.sleep(2)
