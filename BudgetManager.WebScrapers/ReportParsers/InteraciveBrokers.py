import csv
import pandas as pd

with open("..\\BrokerReports\\IB_report.csv", 'r') as file:
    rows = csv.DictReader(file)
    records = []

    filtered_rows = [row for row in rows if row['Statement'] == 'Trades' and row['Field Value'] == 'Stocks' and row['Header'] == 'Data']

    for row in filtered_rows:
        currency = row[None][0]
        ticker = row[None][1]
        date = row[None][2]
        size = row[None][3]
        totalWithoutFee = float(row[None][6])
        total = float(row[None][8])
        buy = totalWithoutFee < 0
        totalWithAction = total * (-1 if buy else 1)

        print('\nTrade data')
        print(currency)
        print(ticker)
        print(date)
        print(size)
        print(totalWithoutFee)
        print(total)
        print(buy)
        print(totalWithAction)
