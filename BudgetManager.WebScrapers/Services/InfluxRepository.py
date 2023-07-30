import logging

import pytz
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS, ASYNCHRONOUS
from datetime import datetime
from typing import List

from Models.FilterTuple import FilterTuple
from Services.InfluxQueryBuilder import InfluxQueryBuilder


def get_datetime_to_log():
    return datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]


class InfluxRepository:
    __client: InfluxDBClient
    __bucket: str
    __entities: List[Point]
    __logger: logging

    def __init__(self, influxUrl: str, bucket: str, token: str, organization: str, logger=None):
        self.__client = InfluxDBClient(url=influxUrl, token=token, org=organization, timeout=30_000)
        self.__bucket = bucket
        self.__entities = []
        self.__logger = logger

    def add(self, point: Point):
        self.__entities.append(point)
        self.__logger and self.__logger.debug(
            'Add point to save (count: ' + str(len(self.__entities)) + '): ' + get_datetime_to_log())

    def add_range(self, points: List[Point]):
        self.__entities.extend(points)
        self.__logger and self.__logger.debug(
            'Add points to save (count: ' + str(len(self.__entities)) + '): ' + get_datetime_to_log())

    def save(self):
        try:
            write_api = self.__client.write_api(write_options=ASYNCHRONOUS)
            self.__logger and self.__logger.debug('START: Influx save ' + get_datetime_to_log())
            write_api.write(self.__bucket, record=self.__entities)
            self.__entities.clear()
            self.__logger and self.__logger.debug('END: Influx save ' + get_datetime_to_log())
        except Exception as e:
            logging.error(e)
            self.__logger and self.__logger.debug('Error while Influx save ' + get_datetime_to_log())
            self.clear_entities()

    def save_batch(self, saveAfter: int = 10):
        try:
            write_api = self.__client.write_api(write_options=ASYNCHRONOUS)
            self.__logger and self.__logger.debug('START: Influx batch save ' + get_datetime_to_log())
            if len(self.__entities) > saveAfter:
                write_api.write(bucket=self.__bucket, record=self.__entities)
                self.__entities.clear()
                self.__logger and self.__logger.debug('END: Influx batch save' + get_datetime_to_log())
        except Exception as e:
            logging.error(e)
            self.__logger and self.__logger.debug('Error while Influx save ' + get_datetime_to_log())
            self.clear_entities()

    def clear_entities(self):
        self.__entities.clear()

    def find_last_for_state_tag(self, measurement: str, tag: str):
        query_api = self.__client.query_api()
        p = {"__start": datetime.MINYEAR,
             "__bucket": self.__bucket,
             "__measurement": measurement,
             "_tag": tag,
             "_desc": True
             }

        tables = query_api.query('''
            from(bucket:__bucket) |> range(start: __start)
                |> filter(fn: (r) => r["__measurement"] == __measurement)
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
        p = {"_tagKey": tagKey, "_measurement": measurement, "__bucket": self.__bucket}

        tables = query_api.query('''
                    from(bucket: __bucket)
                      |> range(start: 1)
                      |> filter(fn: (r) => r["_measurement"] == _measurement)
                      |> keep(columns: [_tagKey])
                      |> distinct(column: _tagKey)
                ''', params=p)

        return tables

    def find_all_last_value_for_tag(self, measurement: str, tagKey: str):
        query_api = self.__client.query_api()
        p = {"_tagKey": tagKey, "_measurement": measurement, "__bucket": self.__bucket}

        tables = query_api.query('''
                    from(bucket: __bucket)
                      |> range(start: 1)
                      |> filter(fn: (r) => r["_measurement"] == _measurement)
                      |> keep(columns: [_tagKey, "_time"])
                      |> sort(columns: ["_time"], desc: true)
                      |> top(n:1, columns: ["_time"])
                ''', params=p)

        return tables

    def filter_last_value(self, measurement: str, ticker: str, start: datetime):
        query_api = self.__client.query_api()

        queryBuilder = InfluxQueryBuilder()
        queryBuilder.set_bucket(self.__bucket)
        queryBuilder.set_start(start)
        queryBuilder.set_end(datetime(2099, 12, 31))
        queryBuilder.set_measurement(measurement)
        queryBuilder.set_filter(FilterTuple("ticker", ticker))
        queryBuilder.set_highestMax("_time")
        query_data = queryBuilder.build()

        tables = query_api.query(query_data[0], params=query_data[1])
        return tables

    def filter_last_value(self, measurement: str, customFilter: FilterTuple, start: datetime):
        query_api = self.__client.query_api()

        queryBuilder = InfluxQueryBuilder()
        queryBuilder.set_bucket(self.__bucket)
        queryBuilder.set_start(start)
        queryBuilder.set_end(datetime(2099, 12, 31))
        queryBuilder.set_measurement(measurement)
        queryBuilder.set_filter(customFilter)
        queryBuilder.set_highestMax("_time")
        query_data = queryBuilder.build()

        tables = query_api.query(query_data[0], params=query_data[1])
        return tables
