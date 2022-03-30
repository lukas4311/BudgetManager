import pyodbc
import requests
import secret
import pandas as pd
from Models.CompanyProfile import CompanyProfile
from secret import fmpApiToken
from typing import List
import datetime


class FmpApiService:
    token: str
    exchanges: List[str] = ["NYSE", "NASDAQ", "AMEX", "EURONEXT"]
    fmpUri: str = "https://financialmodelingprep.com"

    def __init__(self, apiToken: str):
        self.token = apiToken

    def get_company_profile(self, ticker: str):
        response = requests.get(f'{self.fmpUri}/api/v3/profile/{ticker}?apikey={self.token}')
        jsonData = response.json()
        jsonObj = jsonData[0]
        return CompanyProfile.create_from_json(jsonObj)

    def get_historical_dividend(self, ticker: str):
        response = requests.get(f'{self.fmpUri}/api/v3/historical-price-full/stock_dividend/{ticker}?apikey={self.token}')
        print(response.json())

    def get_sector_pe(self, date: datetime):
        response = requests.get(f'{self.fmpUri}/api/v4/sector_price_earning_ratio?date={date.strftime("%m-%d-%Y")}&exchange=NYSE&apikey={self.token}')
        print(response.json())

    def get_sector_performance(self):
        response = requests.get(f'{self.fmpUri}/api/v3/sectors-performance?apikey={self.token}')
        print(response.json())

    def get_sector_performance_history(self, limit: int = 100):
        response = requests.get(f'{self.fmpUri}/api/v3/historical-sectors-performance?limit={limit}&apikey={self.token}')
        print(response.json())

    def get_stock_news(self, tickers: List[str], limit: int = 100):
        joinedTicker = str.join(tickers)
        response = requests.get(f'{self.fmpUri}/api/v3/stock_news?tickers={joinedTicker}&limit={limit}')
        print(response.json())

    def get_stock_news(self, limit: int = 100):
        response = requests.get(f'{self.fmpUri}/api/v3/stock_news?limit={limit}')
        print(response.json())


class FmpScraper:
    fmp_service: FmpApiService

    def __init__(self):
        self.fmp_service = FmpApiService(fmpApiToken)

    def download_profile(self, ticker: str):
        conn = pyodbc.connect(f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
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

            params = (ticker, profile.companyName, profile.currency, profile.isin, profile.exchangeShortName, profile.industry, profile.website, profile.description, profile.sector, profile.image, myTableId)
            cursor.execute('''
                            INSERT INTO [dbo].[CompanyProfile]
                                        ([Symbol],[CompanyName],[Currency],[Isin],[ExchangeShortName],[Industry],[Website],[Description],[Sector],[Image],[AddressId])
                            OUTPUT INSERTED.ID
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                            ''', params)
            conn.commit()


fmpScraper = FmpScraper()
fmpScraper.download_profile("AAPL")
