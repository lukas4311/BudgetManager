import requests
import datetime
import pytz
import pandas as pd
from influxdb_client import Point, WritePrecision


from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton


class MoneyVelocityModel:
    def __init__(self, value, date: datetime, ):
        self.date = date
        self.value = value


def parse_data(fred_txt_link: str):
    data = requests.get(fred_txt_link)
    data_string = str(data.content)
    start_index = data_string.index("VALUE")
    total_assets = list(data_string[start_index + len("VALUE\\r\\n"):].split("\\r\\n"))
    return total_assets


state = "us"
measurement = "MoneyVelocity"

m_models = []
assets_total = parse_data("https://fred.stlouisfed.org/data/M2V.txt")

for assets_data in assets_total:
    split_values = assets_data.split()

    if len(split_values) == 2:
        date_string = split_values[0].split("-")
        dateParsed = datetime.datetime(int(date_string[0]), int(date_string[1]), int(date_string[2]))
        pandas_date = pd.to_datetime(dateParsed)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date = pandas_date.tz_convert("utc")
        m_models.append(MoneyVelocityModel(float(split_values[1]), pandas_date))

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, organizaiton)
min_date = influx_repository.find_last_for_state_tag(measurement, state)

filtered = []

if min_date == datetime.datetime(1971, 1, 1, tzinfo=pytz.utc):
    filtered = m_models
else:
    filtered = list(filter(lambda m: m.date.astimezone(pytz.utc) > min_date, m_models))

for moneyVelocity in filtered:
    point = Point(measurement).field("value", float(moneyVelocity.value)).time(
        moneyVelocity.date, WritePrecision.NS).tag("state", state)
    influx_repository.add(point)

influx_repository.save()
