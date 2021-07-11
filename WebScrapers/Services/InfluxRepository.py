import pytz
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
import datetime


class InfluxRepository:
    __client: InfluxDBClient
    __bucket: str
    __entities: list[Point]

    def __init__(self, influxUrl: str, bucket: str, token: str, organization: str):
        self.__client = InfluxDBClient(url=influxUrl, token=token, org=organization)
        self.__bucket = bucket
        self.__entities = []

    def add(self, point: Point):
        self.__entities.append(point)

    def add_range(self, points: list[Point]):
        self.__entities.extend(points)

    def save(self):
        write_api = self.__client.write_api(write_options=SYNCHRONOUS)

        for entity in self.__entities:
            write_api.write(bucket=self.__bucket, record=entity)

    def find_last(self, measurement: str, tag: str):
        query_api = self.__client.query_api()
        p = {"_start": datetime.MINYEAR,
             "_bucket": self.__bucket,
             "_measurement": measurement,
             "_tag": tag,
             "_desc": True
             }

        tables = query_api.query('''
            from(bucket:_bucket) |> range(start: _start)
                |> filter(fn: (r) => r["_measurement"] == _measurement)
                |> filter(fn: (r) => r["state"] == _tag)
                |> sort(columns: ["_time"], desc: _desc)
                |> top(n:1, columns: ["_time"])
        ''', params=p)

        if len(tables) != 0:
            return tables[0].records[0]["_time"]
        else:
            return datetime.datetime(1971, 1, 1).astimezone(pytz.utc)
