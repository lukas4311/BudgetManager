import csv
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

influx_repository = InfluxRepository("http://localhost:8086", "Comodity", token, organizaiton)
data = influx_repository.test()
# data = data[0].records[0]

for table in data:
    for record in table.records:
        print(record)

print(data)


#
# print(tickers)
# print(len(tickers))
