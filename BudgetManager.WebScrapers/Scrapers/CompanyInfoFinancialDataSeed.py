import csv
import datetime

import pytz
from influxdb_client import Point, WritePrecision
from secret import influxDbUrl
from configManager import token
from configManager import organizaiton
from Services.InfluxRepository import InfluxRepository

tickers = []


def addTickerFromCsvFile(rows):
    for row in rows:
        symbol = row["Symbol"]
        if "^" not in symbol:
            tickers.append(symbol)


# with open("..\\SourceFiles\\nasdaq_screener_1649418624867.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     addTickerFromCsvFile(csv_file)
#
# with open("..\\SourceFiles\\nasdaq_screener_1649418684428.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     addTickerFromCsvFile(csv_file)
#
# with open("..\\SourceFiles\\nasdaq_screener_1649418699710.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     addTickerFromCsvFile(csv_file)
#

influx_repository = InfluxRepository(influxDbUrl, "Stocks", token, organizaiton)
data = influx_repository.find_all_last_value_for_tag("IncomeStatement", "ticker")

for table in data:
    for record in table.records:
        print(record)
