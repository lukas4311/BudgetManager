from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS
import pytz

class MoneySupplyModel:
    def __init__(self, value, date, ):
        self.date = date
        self.value = value
        # self.measurement = measurement
        # self.point = Point(measurement).field("value", float(self.value)).time(self.date.astimezone(pytz.utc), WritePrecision.NS)

    # def addTag(self, tagKey, tagValue):
    #     self.point.tag(tagKey, tagValue)
    #
    # def addField(self, fieldKey, fieldValue):
    #     self.point.field(fieldKey, fieldValue)
    #
    # def save(self, bucket: str):
    #     # bucket = "FinancialIndicators"
    #     client = InfluxDBClient(url="http://localhost:8086", token=, org="8f46f33452affe4a")
    #     write_api = client.write_api(write_options=SYNCHRONOUS)
    #     point = self.point.tag("state", "CZ")
    #     write_api.write(bucket=bucket, record=point)
