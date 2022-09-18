from datetime import datetime
from Models import FilterTuple


class InfluxQueryBuilder:
    __pivotCommand: str = ' |> pivot(rowKey:["_time"], columnKey: ["_field"], valueColumn: "_value")'
    __end: str
    __start: str
    __query: str
    __bucket: str
    __measurement: str
    __conditions: list[str]
    __top: int
    __orderColumns: list[str]
    __orderDesc: bool
    __pivotColumns: bool
    __highestMaxColumn: str

    def __init__(self):
        self.__query = ""
        self.__top = None
        self.__orderColumns = None
        self.__orderDesc = None
        self.__bucket = None
        self.__measurement = None
        self.__start = None
        self.__end = None
        self.__conditions = []
        self.__pivotColumns = False
        self.__highestMaxColumn = None

    def set_bucket(self, bucket: str):
        self.__bucket = bucket

    def set_start(self, start: datetime):
        self.__start = start

    def set_end(self, end: datetime):
        self.__end = end

    def set_measurement(self, measurement: str):
        self.__measurement = measurement

    def set_filter(self, filterItem: FilterTuple):
        self.__conditions.append(filterItem)

    def set_top(self, top: int):
        self.__top = top

    def set_order(self, columns: list[str], desc: bool):
        self.__orderDesc = desc
        self.__orderColumns = columns

    def set_highestMax(self, column: str):
        self.__highestMaxColumn = column

    def set_do_pivot(self):
        self.__pivotColumns = True

    def build(self):
        queryParams = {"_bucket": self.__bucket, "_measurement": self.__measurement}

        self.__query += 'from(bucket: _bucket)'

        if self.__start is not None or self.__end is not None:
            self.__query += ' |> range('

            if self.__start is not None:
                self.__query += 'start: _start'
                queryParams['_start'] = self.__start

            if self.__end is not None:
                if self.__start is not None:
                    self.__query += ','

                self.__query += 'stop: _stop'
                queryParams['_stop'] = self.__end

            self.__query += ')'

        self.__query += ' |> filter(fn: (r) => r["_measurement"] == _measurement)'

        for filterItem in self.__conditions:
            filterName = filterItem.key
            self.__query += f' |> filter(fn: (r) => r["{filterName}"] == _{filterName})'
            queryParams[f'_{filterName}'] = filterItem.value

        if self.__pivotColumns:
            self.__query += self.__pivotCommand

        if self.__top is not None:
            self.__query += ' |> sort(columns: ["_time"], desc: _orderDesc)'
            queryParams['_orderDesc'] = self.__orderDesc

        if self.__top is not None:
            self.__query += ' |> top(n:_top)'
            queryParams['_top'] = self.__top

        if self.__highestMaxColumn is not None:
            self.__query += ' |> highestMax(n:1, column:_highestMaxColumn, groupColumns: [_highestMaxColumn])'
            queryParams["_highestMaxColumn"] = self.__highestMaxColumn

        returnQuery = self.__query
        self.__query = ''
        return returnQuery, queryParams

