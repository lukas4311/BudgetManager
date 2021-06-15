# DOWNLOAD M2 FROM CZ
import pytz
import requests
from bs4 import BeautifulSoup
import datetime
from influxdb_client import Point, WritePrecision
from InfluxRepository import InfluxRepository
from MoneySupplyModel import MoneySupplyModel
from configManager import token

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
page = requests.get(
    "https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=57225&p_uka=8&p_strid=AAAG&p_od=200201&p_do=202104&p_lang=CS&p_format=0&p_decsep=%2C")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
m2Models = []

for tableRow in tableOfValues:
    values = tableRow.findAll('td')
    rawDate = str(values[0].text)
    value = str(values[1].text)
    index = rawDate.index("/")

    date = datetime.datetime(int(rawDate[index + 1:]), int(rawDate[:index]), 1)
    num = float(value.replace(",", "."))
    m2Models.append(MoneySupplyModel(num, date))

minDate = influx_repository.find_last("M2", "cz")

# TODO: need to filter all data older than last record

# for m2 in m2Models:
#     point = Point("M2").field("value", float(m2.value)).time(m2.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", "cz")
#     influx_repository.add(point)
#
# influx_repository.save()
