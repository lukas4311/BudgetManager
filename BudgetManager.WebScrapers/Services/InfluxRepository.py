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

    def __init__(self, influx_url: str, bucket: str, token: str, organization: str, logger=None):
        self.__client = InfluxDBClient(url=influx_url, token=token, org=organization, timeout=30_000)
        self.__bucket = bucket
        self.__entities = []
        self.__logger = logger

    def add(self, point: Point):
        """
        Adds a single InfluxDB Point to the internal entity list for later saving.

        :param point: InfluxDB Point object to be stored.
        """
        self.__entities.append(point)
        self.__logger and self.__logger.debug(
            'Add point to save (count: ' + str(len(self.__entities)) + '): ' + get_datetime_to_log())

    def add_range(self, points: List[Point]):
        """
        Adds multiple InfluxDB Points to the internal entity list for later saving.

        :param points: List of InfluxDB Point objects.
        """
        self.__entities.extend(points)
        self.__logger and self.__logger.debug(
            'Add points to save (count: ' + str(len(self.__entities)) + '): ' + get_datetime_to_log())

    def save(self):
        """
        Synchronously writes all stored points to the InfluxDB bucket and clears the internal list.
        Logs each step and handles any exceptions by logging and clearing points.
        """
        try:
            write_api = self.__client.write_api(write_options=SYNCHRONOUS)
            self.__logger and self.__logger.debug('START: Influx save ' + get_datetime_to_log())
            write_api.write(self.__bucket, record=self.__entities)
            self.__entities.clear()
            self.__logger and self.__logger.debug('END: Influx save ' + get_datetime_to_log())
        except Exception as e:
            logging.error(e)
            self.__logger and self.__logger.debug('Error while Influx save ' + get_datetime_to_log())
            self.clear_entities()

    def save_async(self):
        """
        Asynchronously writes all stored points to the InfluxDB bucket and clears the internal list.
        Logs each step and handles any exceptions by logging and clearing points.
        """
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

    def save_batch(self, save_after: int = 10):
        """
        Asynchronously writes stored points to the InfluxDB bucket if the number of points exceeds the specified threshold.

        :param save_after: Minimum number of points required to trigger a write operation.
        """
        try:
            write_api = self.__client.write_api(write_options=ASYNCHRONOUS)
            self.__logger and self.__logger.debug('START: Influx batch save ' + get_datetime_to_log())
            if len(self.__entities) > save_after:
                write_api.write(bucket=self.__bucket, record=self.__entities)
                self.__entities.clear()
                self.__logger and self.__logger.debug('END: Influx batch save' + get_datetime_to_log())
        except Exception as e:
            logging.error(e)
            self.__logger and self.__logger.debug('Error while Influx save ' + get_datetime_to_log())
            self.clear_entities()

    def save_batch_async(self, save_after: int = 10):
        """
        Synchronously writes stored points to the InfluxDB bucket if the number of points exceeds the specified threshold.

        :param save_after: Minimum number of points required to trigger a write operation.
        """
        try:
            write_api = self.__client.write_api(write_options=SYNCHRONOUS)
            self.__logger and self.__logger.debug('START: Influx batch save ' + get_datetime_to_log())
            if len(self.__entities) > save_after:
                write_api.write(bucket=self.__bucket, record=self.__entities)
                self.__entities.clear()
                self.__logger and self.__logger.debug('END: Influx batch save' + get_datetime_to_log())
        except Exception as e:
            logging.error(e)
            self.__logger and self.__logger.debug('Error while Influx save ' + get_datetime_to_log())
            self.clear_entities()

    def clear_entities(self):
        """
        Clears the internal list of stored points (unsaved data).
        """
        self.__entities.clear()

    def find_last_for_state_tag(self, measurement: str, tag: str):
        """
        Finds the most recent timestamp for a specific 'state' tag value in a measurement.

        :param measurement: Measurement name to search in.
        :param tag: Value of the 'state' tag to filter by.
        :return: Latest timestamp of matching entry, or a default datetime if none found.
        """
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
        """
        Retrieves all distinct values for a specific tag key from a given measurement.

        :param measurement: Measurement name to search in.
        :param tagKey: Tag key to retrieve distinct values for.
        :return: Query result containing all distinct tag values.
        """
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
        """
        Retrieves the last value (latest timestamp) for each distinct value of a tag key.

        :param measurement: Measurement name to search in.
        :param tagKey: Tag key to group by.
        :return: Query result with the latest record for each distinct tag value.
        """
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
        """
        Filters and retrieves the last record for a given ticker starting from a specific datetime.

        :param measurement: Measurement name to search in.
        :param ticker: Value of the 'ticker' tag to filter by.
        :param start: Start datetime of the query range.
        :return: Query result with the latest matching record.
        """
        query_api = self.__client.query_api()

        query_builder = InfluxQueryBuilder()
        query_builder.set_bucket(self.__bucket)
        query_builder.set_start(start)
        query_builder.set_end(datetime(2099, 12, 31))
        query_builder.set_measurement(measurement)
        query_builder.set_filter(FilterTuple("ticker", ticker))
        query_builder.set_highest_max("_time")
        query_data = query_builder.build()

        tables = query_api.query(query_data[0], params=query_data[1])
        return tables

    def filter_last_value(self, measurement: str, custom_filter: FilterTuple, start: datetime):
        """
        Filters and retrieves the last record based on a custom tag-value pair starting from a specific datetime.

        :param measurement: Measurement name to search in.
        :param custom_filter: FilterTuple containing key and value for filtering.
        :param start: Start datetime of the query range.
        :return: Query result with the latest matching record.
        """
        query_api = self.__client.query_api()

        query_builder = InfluxQueryBuilder()
        query_builder.set_bucket(self.__bucket)
        query_builder.set_start(start)
        query_builder.set_end(datetime(2099, 12, 31))
        query_builder.set_measurement(measurement)
        query_builder.set_filter(custom_filter)
        query_builder.set_highest_max("_time")
        query_data = query_builder.build()

        tables = query_api.query(query_data[0], params=query_data[1])
        return tables
