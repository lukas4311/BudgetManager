import pytz
from influxdb_client import Point, WritePrecision
import pandas as pd
from Services.InfluxRepository import InfluxRepository
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

    for pointData in data:
        point = Point('FinSummary')\
            .tag("ticker", ticker).field(pointData.name, float(pointData.value.replace(',', '.').replace('(', '').replace(')', '').replace('%', '')))
        pandas_date: str = None

        if pointData.year != 'TTM':
            pandas_date = pd.to_datetime(f"{pointData.year}-01-01")
            point = point.tag("prediction", "N")
        else:
            now = datetime.now() + relativedelta(years=1)
            now_str = now.strftime("%Y-%m-%d")
            pandas_date = pd.to_datetime(now_str)
            point = point.tag("prediction", "Y")

        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        date = pandas_date.astimezone(pytz.utc)
        point = point.time(date, WritePrecision.NS)
        points.append(point)

    influx_repository.add_range(points)
    influx_repository.save()

# TEST CODE
download_fin_summary('AAPL')