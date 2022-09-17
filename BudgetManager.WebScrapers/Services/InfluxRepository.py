from dataclasses import dataclass
import pytz
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime
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

    def filter_last_value(self, measurement: str, filters: List[FilterTuple], start: datetime):
        query_api = self.__client.query_api()

        queryBuilder = InfluxQueryBuilder()
        queryBuilder.set_bucket(self.__bucket)
        queryBuilder.set_start(start)
        queryBuilder.set_measurement(measurement)
        queryBuilder.set_order(["_time"], True)

        for filterItem in filters:
            queryBuilder.set_filter(filterItem)

        queryBuilder.set_top(1)
        query_data = queryBuilder.build()
        tables = query_api.query(query_data[0], params=query_data[1])
        return tables


class InfluxQueryBuilder:
    _end: str
    _start: str
    _query: str
    _bucket: str
    _measurement: str
    _conditions: list[str]
    _top: int
    _orderColumns: list[str]
    _orderDesc: bool

    def __init__(self):
        self._query = ""
        self._top = None
        self._orderColumns = None
        self._orderDesc = None
        self._bucket = None
        self._measurement = None
        self._start = None
        self._end = None
        self._conditions = []

    def set_bucket(self, bucket: str):
        self._bucket = bucket

    def set_start(self, start: datetime):
        self._start = start
        
    def set_end(self, end: datetime):
        self._end = end

    def set_measurement(self, measurement: str):
        self._measurement = measurement

    def set_filter(self, filterItem: FilterTuple):
        self._conditions.append(filterItem)

    def set_top(self, top: int):
        self._top = top

    def set_order(self, columns: list[str], desc: bool):
        self._orderDesc = desc
        self._orderColumns = columns

    def build(self):
        queryParams = {"_bucket": self._bucket, "_measurement": self._measurement}

        self._query += 'from(bucket: _bucket)'

        if self._start is not None or self._end is not None:
            self._query += ' |> range('

            if self._start is not None:
                self._query += 'start: _start'
                queryParams['_start'] = self._start

            if self._end is not None:
                self._query += 'stop: _stop'
                queryParams['_stop'] = self._end

            self._query += ')'

        self._query += ' |> filter(fn: (r) => r["_measurement"] == _measurement)'

        for filterItem in self._conditions:
            filterName = filterItem.key
            self._query += f' |> filter(fn: (r) => r["{filterName}"] == _{filterName})'
            queryParams[f'_{filterName}'] = filterItem.value

        if self._top is not None:
            self._query += ' |> sort(columns: ["_time"], desc: _orderDesc)'
            queryParams['_orderDesc'] = self._orderDesc

        if self._top is not None:
            self._query += ' |> top(n:_top)'
            queryParams['_top'] = self._top

        returnQuery = self._query
        self._query = ''
        return returnQuery, queryParams

