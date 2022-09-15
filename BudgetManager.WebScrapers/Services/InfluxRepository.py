from dataclasses import dataclass
import pytz
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
import datetime
from typing import List

@dataclass
class FilterTuple:
    key: str
    value: str


class InfluxRepository:
    __client: InfluxDBClient
    __bucket: str
    __entities: List[Point]

    def __init__(self, influxUrl: str, bucket: str, token: str, organization: str):
        self.__client = InfluxDBClient(url=influxUrl, token=token, org=organization)
        self.__bucket = bucket
        self.__entities = []

    def add(self, point: Point):
        self.__entities.append(point)

    def add_range(self, points: List[Point]):
        self.__entities.extend(points)

    def save(self):
        write_api = self.__client.write_api(write_options=SYNCHRONOUS)

        for entity in self.__entities:
            write_api.write(bucket=self.__bucket, record=entity)

    def find_last_for_state_tag(self, measurement: str, tag: str):
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

    def find_all_distincted_tag_values(self, measurement: str, tagKey: str):
        query_api = self.__client.query_api()
        p = {"_tagKey": tagKey, "_measurement": measurement, "_bucket": self.__bucket}

        tables = query_api.query('''
                    from(bucket: _bucket)
                      |> range(start: 1)
                      |> filter(fn: (r) => r["_measurement"] == _measurement)
                      |> keep(columns: [_tagKey])
                      |> distinct(column: _tagKey)
                ''', params=p)

        return tables

    def find_all_last_value_for_tag(self, measurement: str, tagKey: str):
        query_api = self.__client.query_api()
        p = {"_tagKey": tagKey, "_measurement": measurement, "_bucket": self.__bucket}

        tables = query_api.query('''
                    from(bucket: _bucket)
                      |> range(start: 1)
                      |> filter(fn: (r) => r["_measurement"] == _measurement)
                      |> keep(columns: [_tagKey, "_time"])
                      |> sort(columns: ["_time"], desc: true)
                      |> top(n:1, columns: ["_time"])
                ''', params=p)

        return tables

    def filter_last_value(self, measurement: str, filters: List[FilterTuple]):
        query_api = self.__client.query_api()
        queryParams = {"_bucket": self.__bucket, "_measurement": measurement}
        for filter in filters:
            print(filter.key)
            queryParams[filter.key] = filter.value

        print(queryParams)
        tables = query_api.query('''
                            from(bucket: _bucket)
                              |> range(start: 2020-01-01)
                              |> filter(fn: (r) => r["_measurement"] == _measurement)
                              |> filter(fn: (r) => r["_field"] == _field)
                              |> filter(fn: (r) => r["ticker"] == ticker)
                              |> sort(columns: ["_time"], desc: true)
                              |> top(n: 1)
                        ''', params=queryParams)
        return tables


class InfluxQueryBuilder:
    _query: str
    _bucket: str
    _measurement: str
    _filters: list[FilterTuple]
    _top: int
    _orderColumns: list[str]
    _orderDesc: bool

    def __init__(self):
        _query = ""
        _top = None
        _orderColumns: None
        _orderDesc: None

    def set_bucket(self, bucket: str):
        self._bucket = bucket

    def set_measurement(self, measurement: str):
        self._measurement = measurement

    def set_field(self, filter: FilterTuple):
        self._filters.append(filter)

    def set_top(self, top: int):
        self._top = top

    def set_order(self, columns: list[str], desc: bool):
        self._orderDesc = desc
        self._orderColumns = columns


