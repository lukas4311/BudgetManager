import pytz
from influxdb_client import Point, WritePrecision
import pandas as pd
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService
from configManager import token, organizaiton
from secret import influxDbUrl
from datetime import datetime
import pandas as pd
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
            pointData.value.replace(',', '').replace('(', '').replace(')', '').replace('%', '')))
        pandas_date: str

        if pointData.year != 'TTM':
            pandas_date = pd.to_datetime(f"{pointData.year}-12-31")
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
    influx_repository.save()


def isRecordToSave(dataYear:str, fluxYear:int):
    try:
        convertedYear = int(dataYear)
        return convertedYear > fluxYear
    except:
        return False


def download_fin_data(ticker: str):
    data = roic_service.get_main_financial_history(ticker)
    points = []

    date = datetime.min
    data_time = influx_repository.filter_last_value('FinData', date)
    time: datetime = data_time[0].records[0]['_time']
    filtered_data = list(filter(lambda c: isRecordToSave(c.year, time.year), data))

    for pointData in filtered_data:
        point = Point('FinData') \
            .tag("ticker", ticker).field(pointData.name, float(
            pointData.value.replace(',', '').replace('(', '').replace(')', '').replace('%', '')))
        pandas_date: str

        if pointData.year != 'TTM':
            pandas_date = pd.to_datetime(f"{pointData.year}-12-31")
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

    print(points)
    # influx_repository.add_range(points)
    # influx_repository.save()


# TEST CODE
# download_fin_summary('AAPL')
download_fin_data('AAPL')

# TEST filter last value from influx
# filters: list[FilterTuple] = [FilterTuple('_field', 'EBITDA'), FilterTuple('ticker', 'AAPL')]
# date = datetime(2019, 1, 1)
# data = influx_repository.filter_last_value('FinData', filters, date)
# record = data[0].records[0]
# print(data[0].columns)
# print(data[0].records)
# print(record['_time'])
