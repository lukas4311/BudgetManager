import pytz
from influxdb_client import Point, WritePrecision
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService, FinData
from configManager import token, organizaiton
from secret import influxDbUrl
from datetime import datetime
import pandas as pd
import csv
import logging
import time

# logging.basicConfig(level=logging.DEBUG)
logging.basicConfig(filename='app.log', filemode='a', format='%(name)s - %(levelname)s - %(message)s', level=logging.DEBUG)
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


def filterFinancialData(bucketName, ticker, data):
    date = datetime.min
    data_time = influx_repository.filter_last_value(bucketName, ticker, date)
    time: datetime = datetime.min
    filtered_data: list[FinData] = data

    if len(data_time) > 0:
        time: datetime = data_time[0].records[0]['_time']
        filtered_data = list(filter(lambda c: isRecordToSave(c.year, time.year), data))
    return filtered_data


def processFinancialDataPoint(bucketName, pointData, points, ticker):
    point = Point(bucketName) \
        .tag("ticker", ticker).field(pointData.name, float(pointData.value.replace(',', '').replace('(', '').replace(')', '').replace('%', '')))
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
    filtered_data = filterFinancialData(bucketName, ticker, data)

    for pointData in filtered_data:
        processFinancialDataPoint(bucketName, pointData, points, ticker)

    if len(points) > 0:
        logging.info('Saving fin summary about ' + ticker)
        influx_repository.add_range(points)
        influx_repository.save()
    else:
        logging.info('Fin summary already saved ' + ticker)


def download_fin_data(ticker: str):
    bucketName = 'FinData'
    data = roic_service.get_main_financial_history(ticker)
    points = []
    filtered_data = filterFinancialData(bucketName, ticker, data)

    for pointData in filtered_data:
        processFinancialDataPoint(bucketName, pointData, points, ticker)

    if len(points) > 0:
        logging.info('Saving fin data about ' + ticker)
        influx_repository.add_range(points)
        influx_repository.save()
    else:
        logging.info('Fin data already saved ' + ticker)


def download_main_fin(ticker: str):
    bucketName = 'FinMain'
    actualYear = datetime.now().year
    date = datetime.min
    date_time = influx_repository.filter_last_value(bucketName, ticker, date)

    if date_time:
        time: datetime = date_time[0].records[0]['_time']
    else:
        time: datetime = datetime(1, 1, 1)

    if time.year < actualYear:
        mainInfo = roic_service.get_main_summary('AAPL')
        bucketName = bucketName
        point = Point(bucketName) \
            .tag("ticker", ticker)\
            .field('Pe', float(mainInfo.pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
            .field('Fw_Pe', float(mainInfo.fw_pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
            .field('Pe_To_Sp', float(mainInfo.pe_to_sp.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
            .field('Div_Yield', float(mainInfo.div_yield.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))\
            .field('MarketCap', float(mainInfo.market_cap.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', '')))

        now = datetime.now()
        now_str = now.strftime("%Y-%m-%d")
        pandas_date = pd.to_datetime(now_str)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        date = pandas_date.astimezone(pytz.utc)
        point = point.time(date, WritePrecision.NS)

        logging.info('Saving main data about ' + ticker)
        influx_repository.add(point)
        influx_repository.save()
    else:
        logging.info('Main fin data already saved ' + ticker)


# TEST CODE
# download_fin_summary('MSFT')
# download_fin_data('AAPL')
# download_main_fin('AAPL')

sp500 = []


def addTickerFromCsvFile(rows, destination: list):
    for row in rows:
        symbol = row["Symbol"]
        logging.debug('Loaded ticker: ' + symbol)

        if "^" not in symbol:
            destination.append(symbol)


with open("..\\SourceFiles\\sp500.csv", 'r') as file:
    csv_file = csv.DictReader(file)
    addTickerFromCsvFile(csv_file, sp500)


for ticker in sp500:
    logging.debug('Processing of ticker:' + ticker)

    try:
        download_fin_summary(ticker)
        logging.info('Downloaded financial summary for company: ' + ticker)
    except Exception as e:
        logging.info('Error while downloading financial summary for ticker: ' + ticker)
        logging.error(e)

    time.sleep(3)

    try:
        download_fin_data(ticker)
        logging.info('Downloaded financial data for company: ' + ticker)
    except Exception as e:
        logging.info('Error while downloading financial data for ticker: ' + ticker)
        logging.error(e)

    time.sleep(3)

    try:
        download_main_fin(ticker)
        logging.info('Downloaded main financial indicators for company: ' + ticker)
    except Exception as e:
        logging.info('Error while downloading main financial indicators for ticker: ' + ticker)
        logging.error(e)

    time.sleep(3)
