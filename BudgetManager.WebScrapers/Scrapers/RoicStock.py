import pytz
from influxdb_client import Point, WritePrecision
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService, FinData
from configManager import token, organizaiton
from secret import influxDbUrl
from datetime import datetime
import pandas as pd

roic_service = RoicService()
influx_repository = InfluxRepository(influxDbUrl, "StocksRoic", token, organizaiton)


def isRecordToSave(dataYear: str, fluxYear: int):
    try:
        if dataYear == 'TTM':
            dataYear = datetime.now().year

        convertedYear = int(dataYear)
        return convertedYear > fluxYear
    except:
        return False


def filterFinancialData(bucketName, data):
    date = datetime.min
    data_time = influx_repository.filter_last_value(bucketName, date)
    time: datetime = datetime.min
    filtered_data: list[FinData] = data
    if len(data_time) > 0:
        time: datetime = data_time[0].records[0]['_time']
        filtered_data = list(filter(lambda c: isRecordToSave(c.year, time.year), data))
    return filtered_data


def processFinancialDataPoint(bucketName, pointData, points, ticker):
    point = Point(bucketName) \
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


def download_fin_summary(ticker: str):
    bucketName = 'FinSummary'
    data = roic_service.get_fin_summary(ticker)
    points = []
    filtered_data = filterFinancialData(bucketName, data)

    for pointData in filtered_data:
        processFinancialDataPoint(bucketName, pointData, points, ticker)

    print(points)
    # influx_repository.add_range(points)
    # influx_repository.save()


def download_fin_data(ticker: str):
    bucketName = 'FinData'
    data = roic_service.get_main_financial_history(ticker)
    points = []
    filtered_data = filterFinancialData(bucketName, data)

    for pointData in filtered_data:
        processFinancialDataPoint(bucketName, pointData, points, ticker)

    print(points)
    # influx_repository.add_range(points)
    # influx_repository.save()


def download_main_fin(ticker: str):
    #TODO: add loading last value for this ticker

    mainInfo = roic_service.get_main_summary('AAPL')
    bucketName = 'FinMain'
    point = Point(bucketName) \
        .tag("ticker", ticker)\
        .field('Pe', float(mainInfo.pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
        .field('Fw_Pe', float(mainInfo.fw_pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
        .field('Pe_To_Sp', float(mainInfo.pe_to_sp.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
        .field('Div_Yield', float(mainInfo.div_yield.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
        .field('MarketCap', float(mainInfo.market_cap.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))

    now = datetime.now()
    now_str = now.strftime("%Y-%m-%d")
    print(now)
    pandas_date = pd.to_datetime(now_str)
    point = point.tag("prediction", "Y")
    pandas_date = pandas_date.tz_localize("Europe/Prague")
    pandas_date.tz_convert("utc")
    date = pandas_date.astimezone(pytz.utc)
    point = point.time(date, WritePrecision.NS)
    influx_repository.add(point)
    influx_repository.save()

# TEST CODE
# download_fin_summary('AAPL')
# download_fin_data('AAPL')

download_main_fin('AAPL')

# TEST filter last value from influx
# filters: list[F ilterTuple] = [FilterTuple('_field', 'EBITDA'), FilterTuple('ticker', 'AAPL')]
# date = datetime(2019, 1, 1)
# data = influx_repository.filter_last_value('FinData', filters, date)
# record = data[0].records[0]
# print(data[0].columns)
# print(data[0].records)
# print(record['_time'])
