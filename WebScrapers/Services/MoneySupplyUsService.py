import datetime
import pytz
import requests
from influxdb_client import Point, WritePrecision
from Models.MoneySupplyModel import MoneySupplyModel
from Services.InfluxRepository import InfluxRepository


class MoneySupplyUsService:
    state: str = "us"

    def __init__(self, influx_repository: InfluxRepository):
        self.influx_repository = influx_repository

    def download_us_money_supply_data(self, fred_txt_link: str, measurement: str):
        m_models = []

        data = requests.get(fred_txt_link)
        data_string = str(data.content)
        start_index = data_string.index("VALUE")
        values = list(data_string[start_index + len("VALUE\\r\\n"):].split("\\r\\n"))

        for money_data in values:
            split_values = money_data.split()

            if len(split_values) == 2:
                date_string = split_values[0].split("-")
                date = datetime.datetime(int(date_string[0]), int(date_string[1]), int(date_string[2]))
                m_models.append(MoneySupplyModel(float(split_values[1]) * 1000000000, date))

        min_date = self.influx_repository.find_last(measurement, self.state)
        utc = pytz.UTC
        filtered = list(filter(lambda m: utc.localize(m.date) > min_date, m_models))

        self.add_all_to_repository(measurement, filtered)
        self.influx_repository.save()

    def __add_all_to_repository(self, measurement: str, filtered: list[MoneySupplyModel]):
        for moneySupply in filtered:
            point = Point(measurement).field("value", float(moneySupply.value)).time(
                moneySupply.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", self.state)
            self.influx_repository.add(point)
