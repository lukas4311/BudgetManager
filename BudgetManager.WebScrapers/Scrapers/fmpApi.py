import pyodbc
import secret
import pandas as pd
from secret import fmpApiToken
from Services.FmpApiService import FmpApiService


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
