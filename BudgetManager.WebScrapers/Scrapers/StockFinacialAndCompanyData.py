import pytz
from influxdb_client import Point, WritePrecision
from sqlalchemy import create_engine, select, insert
from sqlalchemy.orm import Session
from Models.FilterTuple import FilterTuple
from Services.DB.Orm.EnumItem import Base, EnumItem
from Services.DB.Orm.EnumItemType import EnumItemType
from Scrapers.FmpApi import FmpScraper
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService, FinData
from config import token, organizaiton
from config import influxUrl
from datetime import datetime
import secret
import pandas as pd
import logging

# logging.basicConfig(level=logging.DEBUG)
log_name = 'app.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
roic_service = RoicService()
fmpScraper = FmpScraper()
influx_repository = InfluxRepository(influxUrl, "StocksRoic", token, organizaiton, logging)


class StockScrapeManager:

    def __init__(self, roic_service: RoicService, fmp_scraper: FmpScraper, influx_repo: InfluxRepository):
        self.influx_repository = influx_repo
        self.fmp_scraper = fmp_scraper
        self.roic_service = roic_service

    def is_record_to_save(self, data_year: str, flux_year: int):
        try:
            if data_year == 'TTM':
                data_year = datetime.now().year

            converted_year = int(data_year)
            return converted_year > flux_year
        except:
            return False

    def filter_financial_data(self, bucket_name, ticker, data):
        logging.debug('START: Filter financial data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        date = datetime.min
        data_time = influx_repository.filter_last_value(bucket_name, ticker, date)
        time: datetime = datetime.min
        filtered_data: list[FinData] = data

        if len(data_time) > 0:
            time: datetime = data_time[0].records[0]['_time']
            filtered_data = list(filter(lambda c: self.is_record_to_save(c.year, time.year), data))

        logging.debug('END: Filter financial data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        return filtered_data

    def process_financial_data_point(self, bucket_name, point_data, points, ticker):
        point = Point(bucket_name) \
            .tag("ticker", ticker).field(point_data.name, float(
            point_data.value.replace(',', '').replace('(', '').replace(')', '').replace('%', '')))
        pandas_date: str

        if point_data.year != 'TTM':
            pandas_date = pd.to_datetime(f"{point_data.year}-12-31")
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
        bucket_name = 'FinSummary'
        data = self.roic_service.get_fin_summary(ticker)
        points = []
        filtered_data = self.filter_financial_data(bucket_name, ticker, data)

        for pointData in filtered_data:
            self.process_financial_data_point(bucket_name, pointData, points, ticker)

        if len(points) > 0:
            logging.info('Saving fin summary about ' + ticker)
            logging.debug('START: Save fin summary:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
            self.influx_repository.add_range(points)
            self.influx_repository.save_batch(save_after=100)
            logging.debug('END: Save fin summary:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Fin summary already saved ' + ticker)

    def download_fin_data(self, ticker: str):
        bucket_name = 'FinData'
        data = self.roic_service.get_main_financial_history(ticker)
        points = []
        filtered_data = self.filter_financial_data(bucket_name, ticker, data)

        for pointData in filtered_data:
            self.process_financial_data_point(bucket_name, pointData, points, ticker)

        if len(points) > 0:
            logging.info('Saving fin data about ' + ticker)
            logging.debug('START: Save fin data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
            self.influx_repository.add_range(points)
            self.influx_repository.save_batch(save_after=100)
            logging.debug('END: Save fin data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Fin data already saved ' + ticker)

    def download_main_fin(self, ticker: str):
        bucket_name = 'FinMain'
        actual_year = datetime.now().year
        date = datetime.min
        date_time = self.influx_repository.filter_last_value(bucket_name, FilterTuple("ticker", ticker), date)

        if date_time:
            time: datetime = date_time[0].records[0]['_time']
        else:
            time: datetime = datetime(1, 1, 1)

        if time.year < actual_year:
            main_info = self.roic_service.get_main_summary('AAPL')
            bucket_name = bucket_name
            point = Point(bucket_name) \
                .tag("ticker", ticker) \
                .field('Pe', float(
                main_info.pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Fw_Pe', float(
                main_info.fw_pe.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Pe_To_Sp', float(
                main_info.pe_to_sp.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T', ''))) \
                .field('Div_Yield', float(
                main_info.div_yield.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T',
                                                                                                               ''))) \
                .field('MarketCap', float(
                main_info.market_cap.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T',
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
            self.influx_repository.save_batch(save_after=100)
            logging.debug('END: Save main data:' + datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3])
        else:
            logging.info('Main fin data already saved ' + ticker)

    def store_tickers(self, ticker_shortcut: str, company_name: str):
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItem).where(EnumItem.code == ticker_shortcut)
        ticker_model = session.scalars(stmt).first()

        if ticker_model is None:
            stmt_enum_item_type = select(EnumItemType).where(EnumItemType.code == "TradeTicker")
            enum_item_type = session.scalars(stmt_enum_item_type).first()

            insert_command = insert(EnumItem).values(code=ticker_shortcut, name=company_name, enumItemTypeId=enum_item_type.id)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()

    def store_company_profile(self, ticker: str):
        self.fmpScraper.download_profile(ticker)

    def process_tickers_to_company_profile_to_db(self, rows):
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.store_company_profile(symbol)
            except Exception:
                print(symbol + " - error")

    def process_tickers_to_store_to_db(self, rows):
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.store_tickers(symbol, name)
            except Exception:
                print(symbol + " - error")


def add_ticker_from_csv_file(rows, destination: list):
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
