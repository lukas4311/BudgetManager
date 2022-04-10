import pytz
import requests
import datetime
from bs4 import BeautifulSoup
from influxdb_client import WritePrecision, Point

from Models.InflationAradModel import InflationAradModel
from Services.DatetimeService import parse_arad_datetime_format
from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton


class InterestRateModel:
    def __init__(self, value, date: datetime, ):
        self.date = date
        self.value = value


influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, organizaiton)
page = requests.get(
    "https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=41549&p_uka=3&p_strid=FBC&p_od=199301&p_do=202106&p_lang=CS&p_format=0&p_decsep=%2C")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
interest_rate_models = []

for tableRow in tableOfValues:
    values = tableRow.findAll('td')
    date = parse_arad_datetime_format(str(values[0].text))
    repo_string_value: str = values[1].text

    if repo_string_value != '':
        interest_rate = float(values[1].text.replace(",", "."))
        interest_rate_models.append(InterestRateModel(interest_rate, date))

interestRateMeasurement = "InterestRate"
minDate = influx_repository.find_last_for_state_tag(interestRateMeasurement, "cz")
filtered = list(filter(lambda i: i.date.astimezone(pytz.utc) > minDate, interest_rate_models))

for repo in filtered:
    point = Point(interestRateMeasurement)\
        .field("value", float(repo.value))\
        .time(repo.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", "cz")
    influx_repository.add(point)

influx_repository.save()
