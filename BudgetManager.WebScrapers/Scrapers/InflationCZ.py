import pytz
import requests
from bs4 import BeautifulSoup
from influxdb_client import WritePrecision, Point

from Models.InflationAradModel import InflationAradModel
from Services.DatetimeService import parse_arad_datetime_format
from Services.InfluxRepository import InfluxRepository
from config import token
from config import organizaiton

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, organizaiton)
page = requests.get(
    "https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=21727&p_uka=1%2C5%2C9%2C11%2C13&p_strid=ACBAA&p_od=200701&p_do=202105&p_lang=EN&p_format=0&p_decsep=.")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
inflationModels = []

for tableRow in tableOfValues:
    values = tableRow.findAll('td')
    date = parse_arad_datetime_format(str(values[0].text))
    customerPrice = float(values[1].text.replace(",", "."))
    netInflation = float(values[2].text.replace(",", "."))
    coreInflation = float(values[3].text.replace(",", "."))
    fuelInflation = float(values[4].text.replace(",", "."))
    monetaryRelevantInflation = float(values[5].text.replace(",", "."))
    inflationModels.append(InflationAradModel(date, customerPrice, netInflation, coreInflation, fuelInflation, monetaryRelevantInflation))

minDate = influx_repository.find_last_for_state_tag("Inflation", "cz")
filtered = list(filter(lambda inflation: inflation.date.astimezone(pytz.utc) > minDate, inflationModels))

for inflation in filtered:
    point = Point("Inflation")\
        .field("customerPrice", float(inflation.customerPrice))\
        .field("netInflation", float(inflation.netInflation))\
        .field("coreInflation", float(inflation.coreInflation))\
        .field("fuelInflation", float(inflation.fuelInflation))\
        .field("monetaryRelevantInflation", float(inflation.monetaryRelevantInflation))\
        .time(inflation.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", "cz")
    influx_repository.add(point)

influx_repository.save()
