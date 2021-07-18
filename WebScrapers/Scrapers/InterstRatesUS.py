import requests
import datetime
import pytz
from influxdb_client import Point, WritePrecision

from Services.InfluxRepository import InfluxRepository
from configManager import token


class InterestRateModel:
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
measurement = "InterestRate"

m_models = []
interest_rates = parse_data("https://fred.stlouisfed.org/data/FEDFUNDS.txt")

for interest_rate in interest_rates:
    split_values = interest_rate.split()

    if len(split_values) == 2:
        date_string = split_values[0].split("-")
        dateParsed = datetime.datetime(int(date_string[0]), int(date_string[1]), int(date_string[2]))
        m_models.append(InterestRateModel(float(split_values[1]), dateParsed))

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
min_date = influx_repository.find_last(measurement, state)

filtered = []

if min_date == datetime.datetime(1971, 1, 1).astimezone(pytz.utc):
    filtered = m_models
else:
    filtered = list(filter(lambda m: m.date.astimezone(pytz.utc) > min_date, m_models))

for interest_rate in filtered:
    point = Point(measurement).field("value", float(interest_rate.value)).time(
        interest_rate.date, WritePrecision.NS).tag("state", state)
    influx_repository.add(point)

influx_repository.save()
