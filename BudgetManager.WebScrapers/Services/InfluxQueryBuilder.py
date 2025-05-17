from datetime import datetime
from Models import FilterTuple


class InfluxQueryBuilder:
    """
    Builder class for constructing InfluxDB Flux queries dynamically with filtering, sorting,
    pivoting, and top/max functions.
    """

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
        """Initializes the InfluxQueryBuilder with default values."""
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
        """
        Sets the bucket name to query from.

        Args:
            bucket (str): The name of the InfluxDB bucket.
        """
        self.__bucket = bucket

    def set_start(self, start: datetime):
        """
        Sets the start time for the query range.

        Args:
            start (datetime): Start timestamp.
        """
        self.__start = start

    def set_end(self, end: datetime):
        """
        Sets the end time for the query range.

        Args:
            end (datetime): End timestamp.
        """
        self.__end = end

    def set_measurement(self, measurement: str):
        """
        Sets the measurement name to query.

        Args:
            measurement (str): Name of the measurement (like a table).
        """
        self.__measurement = measurement

    def set_filter(self, filterItem: FilterTuple):
        """
        Adds a filter condition to the query.

        Args:
            filterItem (FilterTuple): A key-value pair used to filter rows.
        """
        self.__conditions.append(filterItem)

    def set_top(self, top: int):
        """
        Limits the result to the top N rows.

        Args:
            top (int): Number of top rows to retrieve.
        """
        self.__top = top

    def set_order(self, columns: list[str], desc: bool):
        """
        Sets the columns to order by, and the sort direction.

        Args:
            columns (list[str]): List of columns to sort by.
            desc (bool): Whether to sort in descending order.
        """
        self.__orderDesc = desc
        self.__orderColumns = columns

    def set_highest_max(self, column: str):
        """
        Finds the row(s) with the highest max value of a specific column.

        Args:
            column (str): Column to evaluate max values from.
        """
        self.__highestMaxColumn = column

    def set_do_pivot(self):
        """
        Enables pivoting of columns in the query (rows become columns by field name).
        """
        self.__pivotColumns = True

    def build(self):
        """
        Builds the final Flux query string along with parameters.

        Returns:
            tuple[str, dict]: A tuple containing:
                - The Flux query string.
                - A dictionary of query parameters.
        """
        query_params = {"_bucket": self.__bucket, "_measurement": self.__measurement}

        self.__query += 'from(bucket: _bucket)'

        if self.__start is not None or self.__end is not None:
            self.__query += ' |> range('

            if self.__start is not None:
                self.__query += 'start: _start'
                query_params['_start'] = self.__start

            if self.__end is not None:
                if self.__start is not None:
                    self.__query += ','
                self.__query += 'stop: _stop'
                query_params['_stop'] = self.__end

            self.__query += ')'

        self.__query += ' |> filter(fn: (r) => r["_measurement"] == _measurement)'

        for filterItem in self.__conditions:
            filter_name = filterItem.key
            self.__query += f' |> filter(fn: (r) => r["{filter_name}"] == _{filter_name})'
            query_params[f'_{filter_name}'] = filterItem.value

        if self.__pivotColumns:
            self.__query += self.__pivotCommand

        if self.__top is not None:
            self.__query += ' |> sort(columns: ["_time"], desc: _orderDesc)'
            query_params['_orderDesc'] = self.__orderDesc

        if self.__top is not None:
            self.__query += ' |> top(n:_top)'
            query_params['_top'] = self.__top

        if self.__highestMaxColumn is not None:
            self.__query += ' |> highestMax(n:1, column:_highestMaxColumn, groupColumns: [_highestMaxColumn])'
            query_params["_highestMaxColumn"] = self.__highestMaxColumn

        return_query = self.__query
        self.__query = ''
        return return_query, query_params
