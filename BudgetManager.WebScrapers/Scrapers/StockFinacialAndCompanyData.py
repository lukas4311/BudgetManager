import pytz
from influxdb_client import Point, WritePrecision
from Scrapers.FmpApi import FmpScraper
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService, FinData
from configManager import token, organizaiton
from secret import influxDbUrl
from datetime import datetime
import secret
import pandas as pd
import csv
import logging
import time
import pyodbc

# logging.basicConfig(level=logging.DEBUG)
log_name = 'app.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
roic_service = RoicService()
fmpScraper = FmpScraper()
influx_repository = InfluxRepository(influxDbUrl, "StocksRoic", token, organizaiton, logging)


class StockScrapeManager:

    def __init__(self, roic_service: RoicService, fmp_scraper: FmpScraper, influx_repo: InfluxRepository):
        self.influx_repository = influx_repo
        self.fmp_scraper = fmp_scraper
        self.roic_service = roic_service

    def isRecordToSave(self, dataYear: str, fluxYear: int):
        try:
            if dataYear == 'TTM':
                dataYear = datetime.now().year

            convertedYear = int(dataYear)
            return convertedYear > fluxYear
        except:
            return False

    def filterFinancialData(self, bucketName, ticker, data):
        logging.debug('START: Filter financial data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        date = datetime.min
        data_time = influx_repository.filter_last_value(bucketName, ticker, date)
        time: datetime = datetime.min
        filtered_data: list[FinData] = data

        if len(data_time) > 0:
            time: datetime = data_time[0].records[0]['_time']
            filtered_data = list(filter(lambda c: self.isRecordToSave(c.year, time.year), data))

        logging.debug('END: Filter financial data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        return filtered_data

    def processFinancialDataPoint(self, bucketName, pointData, points, ticker):
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

    def download_fin_summary(self, ticker: str):
        bucketName = 'FinSummary'
        data = self.roic_service.get_fin_summary(ticker)
        points = []
        filtered_data = self.filterFinancialData(bucketName, ticker, data)

        for pointData in filtered_data:
            self.processFinancialDataPoint(bucketName, pointData, points, ticker)

        if len(points) > 0:
            logging.info('Saving fin summary about ' + ticker)
            logging.debug('START: Save fin summary:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
            self.influx_repository.add_range(points)
            self.influx_repository.save_batch(saveAfter=100)
            logging.debug('END: Save fin summary:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Fin summary already saved ' + ticker)

    def download_fin_data(self, ticker: str):
        bucketName = 'FinData'
        data = self.roic_service.get_main_financial_history(ticker)
        points = []
        filtered_data = self.filterFinancialData(bucketName, ticker, data)

        for pointData in filtered_data:
            self.processFinancialDataPoint(bucketName, pointData, points, ticker)

        if len(points) > 0:
            logging.info('Saving fin data about ' + ticker)
            logging.debug('START: Save fin data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
            self.influx_repository.add_range(points)
            self.influx_repository.save_batch(saveAfter=100)
            logging.debug('END: Save fin data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Fin data already saved ' + ticker)

    def download_main_fin(self, ticker: str):
        bucketName = 'FinMain'
        actualYear = datetime.now().year
        date = datetime.min
        date_time = self.influx_repository.filter_last_value(bucketName, ticker, date)

        if date_time:
            time: datetime = date_time[0].records[0]['_time']
        else:
            time: datetime = datetime(1, 1, 1)

        if time.year < actualYear:
            mainInfo = self.roic_service.get_main_summary('AAPL')
            bucketName = bucketName
            point = Point(bucketName) \
                .tag("ticker", ticker) \
                .field('Pe', float(
                mainInfo.pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Fw_Pe', float(
                mainInfo.fw_pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Pe_To_Sp', float(
                mainInfo.pe_to_sp.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Div_Yield', float(
                mainInfo.div_yield.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T',
                                                                                                               ''))) \
                .field('MarketCap', float(
                mainInfo.market_cap.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T',
                                                                                                                '')))

            now = datetime.now()
            now_str = now.strftime("%Y-%m-%d")
            pandas_date = pd.to_datetime(now_str)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date.tz_convert("utc")
            date = pandas_date.astimezone(pytz.utc)
            point = point.time(date, WritePrecision.NS)

            logging.info('Saving main data about ' + ticker)
            logging.debug('START: Save main data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
            self.influx_repository.add(point)
            self.influx_repository.save_batch(saveAfter=100)
            logging.debug('END: Save main data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Main fin data already saved ' + ticker)

    def storeTickers(self, tickerShortcut: str, companyName: str):
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        sql = """SELECT [Ticker] FROM [dbo].[StockTicker] WHERE [Ticker] = ?"""
        df = pd.read_sql_query(sql, conn, params=[tickerShortcut])

        if len(df.index) == 0:
            cursor = conn.cursor()
            params = (tickerShortcut, companyName)
            cursor.execute('''
                                INSERT INTO [dbo].[StockTicker]([Ticker], [Name])
                                VALUES(?,?)
                            ''', params)
            conn.commit()

    def storeCompanyProfile(self, ticker: str):
        self.fmpScraper.download_profile(ticker)

    def processTickersToCompanyProfileToDb(self, rows):
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.storeCompanyProfile(symbol)
            except Exception:
                print(symbol + " - error")

    def processTickersToStoreToDb(self, rows):
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.storeTickers(symbol, name)
            except Exception:
                print(symbol + " - error")


def addTickerFromCsvFile(rows, destination: list):
    for row in rows:
        symbol = row["Symbol"]
        logging.debug('Loaded ticker: ' + symbol)

        if "^" not in symbol:
            destination.append(symbol)


# sp500 = []
#
# with open("..\\SourceFiles\\sp500.csv", 'r') as file:
#     csv_file = csv.DictReader(file)
#     # processTickersToStoreToDb(csv_file)
#     # addTickerFromCsvFile(csv_file, sp500)
#     stock_scraper = StockScrapeManager(roic_service, fmpScraper, influx_repository)
#     stock_scraper.processTickersToCompanyProfileToDb(csv_file)

# for ticker in sp500:
#     logging.debug('Processing of ticker:' + ticker)
#
#     try:
#         download_fin_summary(ticker)
#         logging.info('Downloaded financial summary for company: ' + ticker)
#     except Exception as e:
#         logging.info('Error while downloading financial summary for ticker: ' + ticker)
#         logging.error(e)
#
#     time.sleep(2)
#
#     try:
#         download_fin_data(ticker)
#         logging.info('Downloaded financial data for company: ' + ticker)
#     except Exception as e:
#         logging.info('Error while downloading financial data for ticker: ' + ticker)
#         logging.error(e)
#
#     time.sleep(2)
#
#     try:
#         download_main_fin(ticker)
#         logging.info('Downloaded main financial indicators for company: ' + ticker)
#     except Exception as e:
#         logging.info('Error while downloading main financial indicators for ticker: ' + ticker)
#         logging.error(e)
#
#     time.sleep(2)
#
# influx_repository.save()
# print('Job is done')
