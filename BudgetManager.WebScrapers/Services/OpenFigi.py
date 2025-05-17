import requests
from dataclasses import dataclass
from typing import List
from config import open_figi_url


@dataclass
class Security:
    """
    Data model representing a financial security returned by the OpenFIGI API.
    """
    figi: str
    name: str
    ticker: str
    exchCode: str
    compositeFIGI: str
    securityType: str
    marketSector: str
    shareClassFIGI: str
    securityType2: str
    securityDescription: str


@dataclass
class SecurityData:
    """
    Wrapper for a list of Security instances.
    """
    data: List[Security]


class OpenFigiService:
    """
    Service for interacting with the OpenFIGI API to fetch security information
    by ticker or ISIN.
    """

    def parse_security_data(self, response: requests.Response) -> SecurityData:
        """
        Parses the JSON response from OpenFIGI into SecurityData.

        :param response: HTTP response object from OpenFIGI API.
        :return: Parsed SecurityData object.
        """
        json_data = response.json()
        securities = [Security(**item) for item in json_data[0]['data']]
        return SecurityData(data=securities)

    def get_ticker_figi_exchange_info(self, ticker) -> Security | None:
        """
        Retrieves security information based on a stock ticker symbol.

        :param ticker: Stock ticker (e.g., 'AAPL').
        :return: Security object containing exchange and FIGI info or None if not found.
        """
        ticker_payload = [{"idType": "TICKER", "idValue": f"{ticker}"}]
        securities_data = self.__do_mapping_request(ticker_payload)
        security_info = securities_data.data[0]
        return security_info

    def get_isin_figi_exchange_info(self, isin: str) -> Security | None:
        """
        Retrieves security information based on an ISIN.

        :param isin: International Securities Identification Number.
        :return: Security object containing exchange and FIGI info or None if not found.
        """
        isin_payload = [{"idType": "ID_ISIN", "idValue": f"{isin}"}]
        securities_data = self.__do_mapping_request(isin_payload)
        security_info = securities_data.data[0]
        return security_info

    def __do_mapping_request(self, payload):
        """
        Sends a mapping request to the OpenFIGI API with the provided payload.

        :param payload: List of ID mappings to be resolved by the API.
        :return: Parsed SecurityData response from the API.
        :raises Exception: If the API call fails with a non-200 status code.
        """
        openfigi_url = f'{open_figi_url}/mapping'
        response = requests.post(url=openfigi_url, json=payload)

        if response.status_code != 200:
            raise Exception('Bad response code {}'.format(str(response.status_code)))

        data = self.parse_security_data(response)
        return data


def main():
    figi = OpenFigiService()
    print(figi.get_ticker_figi_exchange_info('AAPL'))


main()
