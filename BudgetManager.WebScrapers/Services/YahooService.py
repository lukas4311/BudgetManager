import urllib.request
import csv
import io

with urllib.request.urlopen("https://query1.finance.yahoo.com/v7/finance/download/KO?period1=511056000&period2=1673156073&interval=1d&events=history&includeAdjustedClose=true") as url:
    data = url.read().decode()

    rows = csv.DictReader(io.StringIO(data))
    for row in rows:
        close = row["Close"]
        date = row["Date"]
        print(date)
        print(close)
