import requests
from dataclasses import dataclass
from typing import List
from config import open_figi_url


@dataclass
class Security:
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
    data: List[Security]


class OpenFigiService:

    def parse_security_data(self, response: requests.Response) -> SecurityData:
        json_data = response.json()
        securities = [Security(**item) for item in json_data[0]['data']]
        return SecurityData(data=securities)

    def get_ticker_figi_exchange_info(self, ticker) -> Security | None:
        ticker_payload = [{"idType": "TICKER", "idValue": f"{ticker}"}]
        securities_data = self.__do_mapping_request(ticker_payload)
        security_info = securities_data.data[0]
        return security_info

    def get_isin_figi_exchange_info(self, isin: str) -> Security | None:
        isin_payload = [{"idType": "ID_ISIN", "idValue": f"{isin}"}]
        securities_data = self.__do_mapping_request(isin_payload)
        security_info = securities_data.data[0]
        return security_info

    def __do_mapping_request(self, payload):
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
