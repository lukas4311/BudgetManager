import pyodbc
import pytz
from influxdb_client import Point, WritePrecision
from datetime import datetime
import secret
import pandas as pd
from Services.InfluxRepository import InfluxRepository
from secret import fmpApiToken
from Services.FmpApiService import FmpApiService
from configManager import token, organizaiton


class FmpScraper:
    fmp_service: FmpApiService
    influx_repository: InfluxRepository

    def __init__(self):
        self.fmp_service = FmpApiService(fmpApiToken)
        self.influx_repository = InfluxRepository("http://localhost:8086", "Stocks", token, organizaiton)

    def download_profile(self, ticker: str):
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
            myTableId = cursor.fetchone()[0]
            conn.commit()

            params = (
            ticker, profile.companyName, profile.currency, profile.isin, profile.exchangeShortName, profile.industry,
            profile.website, profile.description, profile.sector, profile.image, myTableId)
            cursor.execute('''
                            INSERT INTO [dbo].[CompanyProfile]
                                        ([Symbol],[CompanyName],[Currency],[Isin],[ExchangeShortName],[Industry],[Website],[Description],[Sector],[Image],[AddressId])
                            OUTPUT INSERTED.ID
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                            ''', params)
            conn.commit()

    def download_dividends(self, ticker: str):
        measurement = "Dividends"
        divided_model = self.fmp_service.get_historical_dividend(ticker)
        points = []

        for historical_dividend in divided_model.historical:
            pandas_date = pd.to_datetime(historical_dividend.date)
            pandas_date = pandas_date.tz_localize("Europe/Prague")
            pandas_date = pandas_date.tz_convert("utc")
            point = Point(measurement).field("dividend", float(historical_dividend.dividend)).field("adjDividend", float(historical_dividend.adjDividend)).time(
                pandas_date.astimezone(pytz.utc), WritePrecision.NS).tag("ticker", divided_model.symbol)
            points.append(point)

        self.influx_repository.add_range(points)
        self.influx_repository.save()

    def donwload_sector_performance(self):
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
# fmpScraper.donwload_sector_performance()
