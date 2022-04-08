import csv

tickers = []


def addTickerFromCsvFile(rows):
    for row in csv_file:
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


print(tickers)
print(len(tickers))
