import datetime

import requests
from Services.InfluxRepository import InfluxRepository
from configManager import token

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
data = requests.get("https://fred.stlouisfed.org/data/M2SL.txt")
dataString = str(data.content)
startIndex = dataString.index("VALUE")
print(dataString[startIndex + len("VALUE"):])
values = list(dataString[startIndex + len("VALUE\\r\\n"):].split("\\r\\n"))
for m2Data in values:
    splitValues = m2Data.split()

    if len(splitValues) == 2:
        dateString = splitValues[0].split("-")
        date = datetime.datetime(int(dateString[0]), int(dateString[1]), int(dateString[2]))
        print(date)
        print(float(splitValues[1]))
