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
    """
    Manages the scraping and storage of financial stock data from multiple sources.

    This class orchestrates the process of downloading financial data, filtering it,
    and storing it in InfluxDB and SQL Server databases. It handles financial summaries,
    detailed financial data, and main financial indicators for stock tickers.
    """

    def __init__(self, roic_service: RoicService, fmp_scraper: FmpScraper, influx_repo: InfluxRepository):
        """
        Initialize the StockScrapeManager with required services.

        Args:
            roic_service (RoicService): Service for calculating and retrieving ROIC data
            fmp_scraper (FmpScraper): Financial Modeling Prep API scraper
            influx_repo (InfluxRepository): Repository for InfluxDB operations
        """
        self.influx_repository = influx_repo
        self.fmp_scraper = fmp_scraper
        self.roic_service = roic_service

    def is_record_to_save(self, data_year: str, flux_year: int) -> bool:
        """
        Determines if a financial record should be saved based on year comparison.

        Compares the data year with the flux year to decide if the record is newer
        and should be saved. Handles 'TTM' (Trailing Twelve Months) as current year.

        Args:
            data_year (str): The year of the financial data ('YYYY' or 'TTM')
            flux_year (int): The year from existing flux data
        """
        try:
            if data_year == 'TTM':
                data_year = datetime.now().year

            converted_year = int(data_year)
            return converted_year > flux_year
        except:
            return False

    def filter_financial_data(self, bucket_name: str, ticker: str, data: list[FinData]) -> list[FinData]:
        """
        Filters financial data to include only records newer than existing data.

        Queries the InfluxDB for the last saved record of the given ticker and bucket,
        then filters the input data to include only records with years greater than
        the last saved year.

        Args:
            bucket_name (str): The InfluxDB bucket name to query
            ticker (str): The stock ticker symbol
            data (list[FinData]): List of financial data to filter
        """
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

    def process_financial_data_point(self, bucket_name: str, point_data: FinData, points: list, ticker: str) -> None:
        """
        Processes a single financial data point and converts it to an InfluxDB Point.

        Creates an InfluxDB Point object with proper tags, fields, and timestamp.
        Handles both historical data (with specific years) and TTM (current year) data.
        Cleans numeric values by removing formatting characters.

        Args:
            bucket_name (str): The InfluxDB bucket name for the point
            point_data (FinData): The financial data to process
            points (list): List to append the created Point to
            ticker (str): The stock ticker symbol
        """
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

    def download_fin_summary(self, ticker: str) -> None:
        """
        Downloads and stores financial summary data for a given ticker.

        Retrieves financial summary data from the ROIC service, filters out
        already existing records, and stores new data points in InfluxDB.

        Args:
            ticker (str): The stock ticker symbol to download data for
        """
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

    def download_fin_data(self, ticker: str) -> None:
        """
        Downloads and stores detailed financial data for a given ticker.

        Retrieves main financial history data from the ROIC service, filters out
        already existing records, and stores new data points in InfluxDB.

        Args:
            ticker (str): The stock ticker symbol to download data for
        """
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

    def download_main_fin(self, ticker: str) -> None:
        """
        Downloads and stores main financial indicators for a given ticker.

        Retrieves key financial metrics (PE ratio, dividend yield, market cap, etc.)
        from the ROIC service and stores them in InfluxDB. Only updates if no data
        exists for the current year.

        Args:
            ticker (str): The stock ticker symbol to download data for
        """
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
                main_info.pe_to_sp.replace(',', '').replace('(', '').replace(')', '').replace('%', '').replace('T',
                                                                                                               ''))) \
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

    def store_tickers(self, ticker_shortcut: str, company_name: str) -> None:
        """
        Stores a ticker symbol and company name in the SQL Server database.

        Creates a new EnumItem record with the ticker as code and company name
        if it doesn't already exist in the database. Uses the "TradeTicker" enum type.

        Args:
            ticker_shortcut (str): The stock ticker symbol (e.g., 'AAPL')
            company_name (str): The full company name (e.g., 'Apple Inc.')
        """
        engine = create_engine(
            f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItem).where(EnumItem.code == ticker_shortcut)
        ticker_model = session.scalars(stmt).first()

        if ticker_model is None:
            stmt_enum_item_type = select(EnumItemType).where(EnumItemType.code == "TradeTicker")
            enum_item_type = session.scalars(stmt_enum_item_type).first()

            insert_command = insert(EnumItem).values(code=ticker_shortcut, name=company_name,
                                                     enumItemTypeId=enum_item_type.id)
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()

    def store_company_profile(self, ticker: str) -> None:
        """
        Downloads and stores company profile data for a given ticker.

        Uses the FMP (Financial Modeling Prep) scraper to download detailed
        company profile information for the specified ticker.

        Args:
            ticker (str): The stock ticker symbol to download profile for
        """
        self.fmpScraper.download_profile(ticker)

    def process_tickers_to_company_profile_to_db(self, rows) -> None:
        """
        Processes multiple tickers to download company profiles to database.

        Iterates through a collection of ticker data rows and downloads
        company profile information for each ticker using the FMP scraper.
        Continues processing even if individual tickers fail.

        Args:
            rows: Iterable of row objects containing 'Symbol' and 'Name' keys
                 (typically from CSV DictReader)
        """
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.store_company_profile(symbol)
            except Exception:
                print(symbol + " - error")

    def process_tickers_to_store_to_db(self, rows) -> None:
        """
        Processes multiple tickers to store ticker information in database.

        Iterates through a collection of ticker data rows and stores
        each ticker symbol and company name in the SQL Server database.
        Continues processing even if individual tickers fail.

        Args:
            rows: Iterable of row objects containing 'Symbol' and 'Name' keys
                 (typically from CSV DictReader)
        """
        for row in rows:
            symbol = row["Symbol"]
            name = row["Name"]

            try:
                self.store_tickers(symbol, name)
            except Exception:
                print(symbol + " - error")


def add_ticker_from_csv_file(rows, destination: list) -> None:
    """
    Loads ticker symbols from CSV rows and adds them to a destination list.

    Processes CSV rows to extract ticker symbols and adds them to the provided
    list, filtering out symbols that contain the '^' character (typically
    index symbols).

    Args:
        rows: Iterable of row objects containing 'Symbol' key (typically from CSV DictReader)
        destination (list): List to append valid ticker symbols to
    """
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
