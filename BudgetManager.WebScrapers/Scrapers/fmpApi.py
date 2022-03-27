import requests
import secret
from secret import fmpApiToken
from typing import List
import datetime
from dataclasses import dataclass
import pyodbc


@dataclass
class CompanyProfile:
    symbol: str
    price: float
    beta: float
    volAvg: int
    mktCap: float
    lastDiv: float
    range: str
    changes: float
    companyName: str
    currency: str
    cik: str
    isin: str
    cusip: str
    exchange: str
    exchangeShortName: str
    industry: str
    website: str
    description: str
    ceo: str
    sector: str
    country: str
    fullTimeEmployees: str
    phone: str
    address: str
    city: str
    state: str
    zip: str
    dcfDiff: float
    dcf: float
    image: str
    ipoDate: str
    defaultImage: bool
    isEtf: bool
    isActivelyTrading: bool
    isAdr: bool
    isFund: bool

    @staticmethod
    def create_from_json(data):
        return CompanyProfile(**data)


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

    def download_profile(self):
        conn = pyodbc.connect(f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={secret.serverName};DATABASE={secret.datebaseName};Trusted_Connection=yes;')
        cursor = conn.cursor()
        cursor.execute('''
                        INSERT INTO CompanyProfile ([Name], [Currency])
                        VALUES
                        ('Apple','USD'),
                        ('Nio','USD')
                        ''')
        conn.commit()

        # profile = self.fmp_service.get_company_profile("AAPL")
        # print(profile.companyName)
        # print(profile.description)
        # print(profile.exchangeShortName)
        # print(profile.state)
        # print(profile.sector)
        # print(profile.ceo)
        # print(profile.address)
        # print(profile.image)
        # print(profile.defaultImage)
        # print(profile.isin)
        # print(profile.currency)
        # print(profile.industry)
        # print(profile.city)
        # print(profile.country)
        # print(profile.symbol)
        # print(profile.website)
        #
        # print(profile.price)
        # print(profile.volAvg)
        # print(profile.mktCap)


fmpScraper = FmpScraper()
fmpScraper.download_profile()
