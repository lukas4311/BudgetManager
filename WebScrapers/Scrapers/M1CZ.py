import pytz
import requests
from bs4 import BeautifulSoup
import datetime

from influxdb_client import Point, WritePrecision

from Services.InfluxRepository import InfluxRepository
from Models.MoneySupplyModel import MoneySupplyModel
from configManager import token

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
page = requests.get(
    "https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=40683&p_uka=1&p_strid=AAAG&p_od=200201&p_do=202104&p_lang=CS&p_format=0&p_decsep=%2C")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
m1Models = []

for tableRow in tableOfValues:
    values = tableRow.findAll('td')
    rawDate = str(values[0].text)
    value = str(values[1].text)
    index = rawDate.index("/")

    date = datetime.datetime(int(rawDate[index + 1:]), int(rawDate[:index]), 1)
    num = float(value.replace(",", "."))
    m1Models.append(MoneySupplyModel(num, date))

minDate = influx_repository.find_last("M1", "cz")
filtered = list(filter(lambda number: number.date.astimezone(pytz.utc) > minDate, m1Models))

for m1 in filtered:
    point = Point("M1").field("value", float(m1.value)).time(m1.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", "cz")
    influx_repository.add(point)

influx_repository.save()
