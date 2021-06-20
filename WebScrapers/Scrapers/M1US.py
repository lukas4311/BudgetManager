import datetime

import pytz
import requests
from influxdb_client import Point, WritePrecision

from Models.MoneySupplyModel import MoneySupplyModel
from Services.InfluxRepository import InfluxRepository
from configManager import token
m2Models = []

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
data = requests.get("https://fred.stlouisfed.org/data/M1NS.txt")
dataString = str(data.content)
startIndex = dataString.index("VALUE")
values = list(dataString[startIndex + len("VALUE\\r\\n"):].split("\\r\\n"))

for m2Data in values:
    splitValues = m2Data.split()

    if len(splitValues) == 2:
        dateString = splitValues[0].split("-")
        date = datetime.datetime(int(dateString[0]), int(dateString[1]), int(dateString[2]))
        m2Models.append(MoneySupplyModel(float(splitValues[1]) * 1000000000, date))

minDate = influx_repository.find_last("M1", "us")
utc = pytz.UTC
filtered = list(filter(lambda m: utc.localize(m.date) > minDate, m2Models))

for m2 in filtered:
    point = Point("M1").field("value", float(m2.value)).time(m2.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", "us")
    influx_repository.add(point)

influx_repository.save()
