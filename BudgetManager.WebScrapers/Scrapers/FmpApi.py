import pyodbc
import pytz
from influxdb_client import Point, WritePrecision
from datetime import datetime
import secret
import pandas as pd
from Services.InfluxRepository import InfluxRepository
from secret import fmpApiToken
from Services.FmpApiService import FmpApiService
from config import token, organizaiton, influxUrl


class FmpScraper:
    """
    A service class for scraping financial data from Financial Modeling Prep (FMP) API
    and storing it in SQL Server database and InfluxDB.

    This class handles downloading and storing:
    - Company profiles
    - Historical dividend data
    - Sector performance data

    Attributes:
        fmp_service (FmpApiService): Service for interacting with FMP API
        influx_repository (InfluxRepository): Repository for InfluxDB operations
    """
    fmp_service: FmpApiService
    influx_repository: InfluxRepository

    def __init__(self):
        """
        Initialize the FmpScraper with necessary services.

        Creates instances of FmpApiService and InfluxRepository using
        credentials and configuration from imported modules.
        """
        self.fmp_service = FmpApiService(fmpApiToken)
        self.influx_repository = InfluxRepository(influxUrl, "Stocks", token, organizaiton)

    def download_profile(self, ticker: str):
        """
        Download and store company profile data for a given ticker symbol.

        Checks if the company profile already exists in the database. If not,
        fetches the profile from FMP API and stores it in SQL Server database
        along with address information.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'GOOGL')
        """
        conn = pyodbc.connect(
            f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        sql = """SELECT [Id], [CompanyName] FROM [dbo].[CompanyProfile] WHERE [Symbol] = ?"""
        df = pd.read_sql_query(sql, conn, params=[ticker])

        if len(df.index) == 0:
            profile = self.fmp_service.get_company_profile(ticker)
            cursor = conn.cursor()
            params = (profile.country, profile.city, profile.address, profile.state)
            cursor.execute('''
                    INSERT INTO [dbo].[Address]([Country],[City],[Street],[State])
                    OUTPUT INSERTED.ID
                    VALUES(?, ?, ?, ?)
                ''', params)
            my_table_id = cursor.fetchone()[0]
            conn.commit()

            params = (
                ticker, profile.companyName, profile.currency, profile.isin, profile.exchangeShortName,
                profile.industry,
                profile.website, profile.description, profile.sector, profile.image, my_table_id)
            cursor.execute('''
                            INSERT INTO [dbo].[CompanyProfile]
                                        ([Symbol],[CompanyName],[Currency],[Isin],[ExchangeShortName],[Industry],[Website],[Description],[Sector],[Image],[AddressId])
                            OUTPUT INSERTED.ID
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                            ''', params)
            conn.commit()

    def download_dividends(self, ticker: str):
        """
        Download historical dividend data for a ticker and store in InfluxDB.

        Fetches historical dividend data from FMP API, converts timestamps to UTC,
        and stores the data as time-series points in InfluxDB.

        Args:
            ticker (str): Stock ticker symbol for which to download dividend data

        Process:
            1. Fetches historical dividend data from FMP API
            2. Converts dates from Europe/Prague timezone to UTC
            3. Creates InfluxDB points with dividend and adjusted dividend values
            4. Stores all points in InfluxDB using batch operation
        """
        measurement = "Dividends"
        divided_model = self.fmp_service.get_historical_dividend(ticker)
        points = []

        for historical_dividend in divided_model.historical:
            pandas_date = pd.to_datetime(historical_dividend.date)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date = pandas_date.tz_convert("utc")
            point = Point(measurement).time(pandas_date.astimezone(pytz.utc), WritePrecision.NS).tag("ticker",
                                                                                                     divided_model.symbol) \
                .field("dividend", float(historical_dividend.dividend)).field("adjDividend",
                                                                              float(historical_dividend.adjDividend))
            points.append(point)

        self.influx_repository.add_range(points)
        self.influx_repository.save()

    def download_sector_performance(self):
        """
        Download current sector performance data and store in InfluxDB.

        Fetches real-time sector performance data from FMP API and stores it
        as a single time-series point in InfluxDB with current timestamp.

        Process:
            1. Fetches sector performance data from FMP API
            2. Creates a single InfluxDB point with current UTC timestamp
            3. Adds each sector's percentage change as a field
            4. Stores the point in InfluxDB
        """
        measurement = "SectorPerformance"
        sector_models = self.fmp_service.get_sector_performance()
        point = Point(measurement).time(datetime.utcnow(), WritePrecision.NS)

        for sector_data in sector_models:
            percent_change = float(sector_data.changesPercentage.replace('%', ''))
            point.field(sector_data.sector, percent_change)

        self.influx_repository.add(point)
        self.influx_repository.save()


fmpScraper = FmpScraper()
# fmpScraper.download_profile("AAPL")
# fmpScraper.download_dividends("AAPL")

# Every day job
# fmpScraper.download_sector_performance()