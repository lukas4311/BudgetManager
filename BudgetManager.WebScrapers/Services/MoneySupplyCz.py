import datetime
import pytz
from bs4 import ResultSet
from influxdb_client import Point, WritePrecision
from Models.MoneySupplyModel import MoneySupplyModel
from Services.InfluxRepository import InfluxRepository


class MoneySupplyCz:
    def __init__(self, influx_repository: InfluxRepository):
        self.influx_repository = influx_repository

    def download_money_supply_data(self, measurement: str, tableOfValues: ResultSet):
        money_supply_models = []

        for tableRow in tableOfValues:
            values = tableRow.findAll('td')
            raw_date = str(values[0].text)
            value = str(values[1].text)
            index = raw_date.index("/")

            date = datetime.datetime(int(raw_date[index + 1:]), int(raw_date[:index]), 1)
            num = float(value.replace(",", "."))
            money_supply_models.append(MoneySupplyModel(num, date))

        min_date = self.influx_repository.find_last_for_state_tag(measurement, "cz")
        filtered = list(filter(lambda number: number.date.astimezone(pytz.utc) > min_date, money_supply_models))

        for m1 in filtered:
            point = Point(measurement).field("value", float(m1.value)).time(m1.date.astimezone(pytz.utc),
                                                                            WritePrecision.NS).tag("state", "cz")
            self.influx_repository.add(point)

        self.influx_repository.save()
