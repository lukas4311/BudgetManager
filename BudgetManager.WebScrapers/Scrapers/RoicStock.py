import pytz
from influxdb_client import Point, WritePrecision
import pandas as pd
from Services.InfluxRepository import InfluxRepository, FilterTuple
from Services.RoicService import RoicService
from configManager import token, organizaiton
from secret import influxDbUrl
from datetime import datetime
from dateutil.relativedelta import relativedelta

roic_service = RoicService()
influx_repository = InfluxRepository(influxDbUrl, "StocksRoic", token, organizaiton)


def download_fin_summary(ticker: str):
    data = roic_service.get_fin_summary(ticker)
    print(data)
    points = []

    # TODO: get data from timeseries and compare dates which one to save and which one to omit saving

    for pointData in data:
        point = Point('FinSummary') \
            .tag("ticker", ticker).field(pointData.name, float(
            pointData.value.replace(',', '.').replace('(', '').replace(')', '').replace('%', '')))
        pandas_date: str = None

        if pointData.year != 'TTM':
            pandas_date = pd.to_datetime(f"{pointData.year}-31-12")
            point = point.tag("prediction", "N")
        else:
            now = datetime.now()
            now_str = now.strftime("%Y-12-31")
            pandas_date = pd.to_datetime(now_str)
            point = point.tag("prediction", "Y")

        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        date = pandas_date.astimezone(pytz.utc)
        point = point.time(date, WritePrecision.NS)
        points.append(point)

    influx_repository.add_range(points)
    # influx_repository.save()


# TEST CODE
# download_fin_summary('AAPL')


filters: list[FilterTuple] = [FilterTuple('_field', 'Revenue per share'), FilterTuple('ticker', 'AAPL')]
influx_repository.filter_last_value('FinSummary', filters)
