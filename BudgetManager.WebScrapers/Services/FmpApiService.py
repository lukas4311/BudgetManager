import requests
from Models.CompanyProfile import CompanyProfile
from typing import List
from Models.Fmp.Dividends import Dividends
from Models.Fmp.SectorsPerformanceModel import SectorsPerformanceModel
from config import fmpUri

class FmpApiService:
    token: str
    exchanges: List[str] = ["NYSE", "NASDAQ", "AMEX", "EURONEXT"]

    def __init__(self, api_token: str):
        """
        Initializes the FmpApiService with the provided API token.

        :param api_token: API key for accessing FMP endpoints.
        """
        self.token = api_token

    def get_company_profile(self, ticker: str) -> CompanyProfile:
        """
        Retrieves the company profile for a given stock ticker.

        :param ticker: The stock ticker symbol (e.g., "AAPL").
        :return: A CompanyProfile object with company details.
        """
        response = requests.get(f'{fmpUri}/api/v3/profile/{ticker}?apikey={self.token}')
        json_data = response.json()
        json_obj = json_data[0]
        return CompanyProfile.create_from_json(json_obj)

    def get_historical_dividend(self, ticker: str) -> Dividends:
        """
        Retrieves the historical dividend data for a given stock ticker.

        :param ticker: The stock ticker symbol (e.g., "AAPL").
        :return: A Dividends object containing historical dividend information.
        """
        response = requests.get(
            f'{fmpUri}/api/v3/historical-price-full/stock_dividend/{ticker}?apikey={self.token}')
        json_data = response.json()
        return Dividends.create_from_json(json_data)

    def get_sector_performance(self) -> list[SectorsPerformanceModel]:
        """
        Retrieves the sector performance.

        :return: A list of models with sector performance.
        """
        response = requests.get(f'{fmpUri}/api/v3/sectors-performance?apikey={self.token}')
        json_data = response.json()
        # jsonData = [{"sector":"BasicMaterials","changesPercentage":"0.9725%"},{"sector":"CommunicationServices","changesPercentage":"0.8804%"},{"sector":"ConsumerCyclical","changesPercentage":"0.5338%"},{"sector":"ConsumerDefensive","changesPercentage":"0.9416%"},{"sector":"Energy","changesPercentage":"1.6889%"},{"sector":"FinancialServices","changesPercentage":"0.3667%"},{"sector":"Healthcare","changesPercentage":"0.6816%"},{"sector":"Industrials","changesPercentage":"0.6143%"},{"sector":"RealEstate","changesPercentage":"0.3742%"},{"sector":"Technology","changesPercentage":"0.7689%"},{"sector":"Utilities","changesPercentage":"0.3905%"}]
        models = []

        for sector_data in json_data:
            sector_model = SectorsPerformanceModel.create_from_json(sector_data)
            models.append(sector_model)

        return models
