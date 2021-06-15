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

    def add(self, point: Point):
        self.__entities.append(point)

    def add_range(self, points: list[Point]):
        self.__entities.extend(points)

    def save(self):
        write_api = self.__client.write_api(write_options=SYNCHRONOUS)

        for entity in self.__entities:
            write_api.write(bucket=self.__bucket, record=entity)

    def find_last(self, measurement: str):
        query_api = self.__client.query_api()
        p = {"_start": datetime.MINYEAR,
             "_bucket": self.__bucket,
             "_desc": True,
             "_measurement": measurement,
             "_every": datetime.timedelta(minutes=5)
             }

        tables = query_api.query('''
            from(bucket:_bucket) |> range(start: _start)
                |> filter(fn: (r) => r["_measurement"] == "_measurement")
                |> sort(columns: ["_time"], desc: _desc)
                |> top(1)
        ''', params=p)

        for table in tables:
            print(table)
            for record in table.records:
                print(str(record["_time"]))
