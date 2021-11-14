import requests
import datetime
import pytz
from influxdb_client import Point, WritePrecision

from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton


class AssetsModel:
    def __init__(self, value, date, ):
        self.date = date
        self.value = value


def parse_data(fred_txt_link: str):
    data = requests.get(fred_txt_link)
    data_string = str(data.content)
    start_index = data_string.index("VALUE")
    total_assets = list(data_string[start_index + len("VALUE\\r\\n"):].split("\\r\\n"))
    return total_assets


state = "us"

m_models = []
assets_total = parse_data("https://fred.stlouisfed.org/data/WALCL.txt")

for assets_data in assets_total:
    split_values = assets_data.split()

    if len(split_values) == 2:
        date_string = split_values[0].split("-")
        date = datetime.datetime(int(date_string[0]), int(date_string[1]), int(date_string[2]))
        m_models.append(AssetsModel(float(split_values[1]), date))

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, organizaiton)
min_date = influx_repository.find_last("TotalAssets", state)
filtered = list(filter(lambda m: m.date.astimezone(pytz.utc) > min_date, m_models))

for moneySupply in filtered:
    point = Point("TotalAssets").field("value", float(moneySupply.value)).time(
        moneySupply.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", state)
    influx_repository.add(point)

influx_repository.save()
