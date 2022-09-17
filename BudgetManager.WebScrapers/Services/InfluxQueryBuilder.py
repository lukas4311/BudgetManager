from datetime import datetime
from Models import FilterTuple


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

