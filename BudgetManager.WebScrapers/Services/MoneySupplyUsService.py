import datetime
import pytz
import requests
from influxdb_client import Point, WritePrecision
from Models.MoneySupplyModel import MoneySupplyModel
from Services.InfluxRepository import InfluxRepository
from typing import List


class MoneySupplyUsService:
    state: str = "us"

    def __init__(self, influx_repository: InfluxRepository):
        """
        Initializes the MoneySupplyUsService with the provided InfluxRepository instance.

        :param influx_repository: Instance of InfluxRepository used to store data in InfluxDB.
        """
        self.influx_repository = influx_repository

    def download_us_money_supply_data(self, fred_txt_link: str, measurement: str):
        """
        Downloads, parses, filters, and stores US money supply data from the FRED text data source.

        :param fred_txt_link: URL to the FRED money supply text data.
        :param measurement: Name of the InfluxDB measurement to store data in.
        """
        m_models = []
        values = self.__parse_money_supply_data(fred_txt_link)

        for money_data in values:
            split_values = money_data.split()
            self.__parse_data_from_string_array_to_money_supply_model(m_models, split_values)

        min_date = self.influx_repository.find_last_for_state_tag(measurement, self.state)
        utc = pytz.UTC
        filtered = list(filter(lambda m: utc.localize(m.date) > min_date, m_models))

        self.__add_all_to_repository(measurement, filtered)
        self.influx_repository.save()

    def __parse_data_from_string_array_to_money_supply_model(self, m_models, split_values):
        """
        Converts a string array of date and value into a MoneySupplyModel and appends it to the list.

        :param m_models: List to append the parsed MoneySupplyModel into.
        :param split_values: List containing a date string and a numeric string.
        """
        if len(split_values) == 2:
            date_string = split_values[0].split("-")
            date = datetime.datetime(int(date_string[0]), int(date_string[1]), int(date_string[2]))
            m_models.append(MoneySupplyModel(float(split_values[1]) * 1000000000, date))

    def __parse_money_supply_data(self, fred_txt_link):
        """
        Downloads the raw text data from FRED and extracts the relevant lines.

        :param fred_txt_link: URL to the FRED money supply text data.
        :return: List of data strings starting from the "VALUE" section.
        """
        data = requests.get(fred_txt_link)
        data_string = str(data.content)
        start_index = data_string.index("VALUE")
        values = list(data_string[start_index + len("VALUE\\r\\n"):].split("\\r\\n"))
        return values

    def __add_all_to_repository(self, measurement: str, filtered: List[MoneySupplyModel]):
        """
        Converts filtered MoneySupplyModels to InfluxDB Points and adds them to the repository.

        :param measurement: Name of the InfluxDB measurement.
        :param filtered: List of MoneySupplyModel instances to store.
        """
        for moneySupply in filtered:
            point = Point(measurement).field("value", float(moneySupply.value)).time(
                moneySupply.date.astimezone(pytz.utc), WritePrecision.NS).tag("state", self.state)
            self.influx_repository.add(point)
