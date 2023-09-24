import csv
import pandas as pd
from datetime import datetime
from dataclasses import dataclass

from Services.YahooService import YahooService


@dataclass
class IBReportData:
    time: datetime
    ticker: str
    size: float
    total: float
    currency: str
    name: str


yahoo_service = YahooService()
cachedNames = {}

with open("..\\BrokerReports\\IB_report.csv", 'r') as file:
    rows = csv.DictReader(file)
    ib_records = []

    filtered_rows = [row for row in rows if row['Statement'] == 'Trades' and row['Field Value'] == 'Stocks' and row['Header'] == 'Data']

    for row in filtered_rows:
        currency = row[None][0]
        ticker = row[None][1]
        date = row[None][2]
        date = date.split(',')[0]
        size = row[None][3]
        totalWithoutFee = float(row[None][6])
        total = float(row[None][8])
        buy = totalWithoutFee < 0
        totalWithAction = total * (-1 if buy else 1)

        cachedName = cachedNames[ticker] if ticker in cachedNames else None

        if cachedName == None:
            companyName = yahoo_service.get_company_name(ticker)
            name = companyName if companyName != None else ticker
            cachedNames[ticker] = name

        pandas_date = pd.to_datetime(date)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")

        print('\nTrade data')
        model = IBReportData(pandas_date, ticker, size, totalWithAction, currency, name)
        ib_records.append(model)
        print(model)
        # parsedData.append(model)
