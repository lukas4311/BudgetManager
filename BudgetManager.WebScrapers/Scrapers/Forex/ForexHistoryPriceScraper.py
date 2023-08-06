import requests
from itertools import combinations
import pandas as pd
from dataclasses import dataclass
from secret import tokenTwelveData
import json


@dataclass
class PriceModel:
    datetime: pd.Timestamp
    close_price: float
    symbol: str


class TwelveDataUrlBuilder:
    @staticmethod
    def build_time_series_url(symbols):
        base_url = "https://api.twelvedata.com/time_series"
        symbol_param = ','.join(symbols)
        interval = "1day"
        api_key = tokenTwelveData
        return f"{base_url}?symbol={symbol_param}&interval={interval}&apikey={api_key}"


class ApiDataSource:
    def __init__(self, api_url):
        self.api_url = api_url

    def fetch_data(self):
        response = requests.get(self.api_url)
        if response.status_code != 200:
            raise ValueError("Failed to fetch data from the API.")
        return response.json()

    def parse_data(self, data):
        parsed_data = []
        for symbol, symbol_data in data.items():
            if 'values' not in symbol_data:
                continue

            for entry in symbol_data['values']:
                datetime_str = entry['datetime']
                close_price = entry['close']

                datetime = pd.to_datetime(datetime_str)

                model = PriceModel(datetime=datetime, close_price=round(float(close_price), 6), symbol=symbol)
                parsed_data.append(model)

        return parsed_data


class ForexRateAnalyzer:
    def find_all_cross_rates(self, price_data: dict) -> dict:
        cross_data = {}

        for key1, value1 in price_data.items():
            for key2, value2 in price_data.items():
                # Skip if the keys are the same or the reverse order has been calculated
                if key1 == key2 or key2 + "/" + key1 in price_data:
                    continue
                # Create a new key for the cross pair
                cross_key = key1 + "/" + key2
                # Zip the corresponding values by date
                cross_value = list(zip(value1, value2))
                # Loop over the zipped values
                for v1, v2 in cross_value:
                    # Check if the dates match
                    if v1.datetime == v2.datetime:
                        # Calculate the cross rate by dividing the exchange rates
                        cross_rate = round(v1.close_price / v2.close_price, 6)
                        # Create a new PriceModel object with the cross rate and the cross key
                        cross_model = PriceModel(v1.datetime, cross_rate, cross_key)
                        # Split the cross key by the slash and take the first and last elements as the base and quote currencies
                        base_currency = cross_key.split("/")[1]
                        quote_currency = cross_key.split("/")[-1]
                        # Join them with a slash and assign it to the symbol attribute of the cross model
                        key = quote_currency + "/" + base_currency
                        cross_model.symbol = key
                        # Check if the cross key is already in the cross data dictionary
                        if key in cross_data:
                            cross_data[key].append(cross_model)
                        else:
                            cross_data[key] = [cross_model]

        return cross_data
        # Print the cross data dictionary
        # for key, value in cross_data.items():
        #     print(key)
        #     for v in value:
        #         print(v.datetime, v.close_price, v.symbol)


class ForexScrapeService:
    def __init__(self):
        self.data_source = None

    def set_data_source(self, data_source):
        self.data_source = data_source

    def get_data(self, symbols):
        if not self.data_source:
            raise ValueError("Data source is not set. Please call set_data_source() first.")

        api_url = TwelveDataUrlBuilder.build_time_series_url(symbols)
        self.data_source.api_url = api_url
        #data = self.data_source.fetch_data()
        data = '{"USD/CZK":{"meta":{"symbol":"USD/CZK","interval":"1day","currency_base":"US Dollar","currency_quote":"Czech Koruna","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"22.11000","high":"22.21170","low":"21.94720","close":"22.03530"},{"datetime":"2023-08-03","open":"21.91590","high":"22.21390","low":"21.87700","close":"22.13430"},{"datetime":"2023-08-02","open":"21.80990","high":"21.97790","low":"21.71790","close":"21.91560"},{"datetime":"2023-08-01","open":"21.72250","high":"21.86480","low":"21.69340","close":"21.81840"}],"status":"ok"},"USD/EUR":{"meta":{"symbol":"USD/EUR","interval":"1day","currency_base":"US Dollar","currency_quote":"Euro","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.91334","high":"0.91448","low":"0.90568","close":"0.90820"},{"datetime":"2023-08-03","open":"0.91424","high":"0.91629","low":"0.91229","close":"0.91331"},{"datetime":"2023-08-02","open":"0.91043","high":"0.91578","low":"0.90746","close":"0.91420"},{"datetime":"2023-08-01","open":"0.90930","high":"0.91302","low":"0.90870","close":"0.91035"}],"status":"ok"},"USD/GBP":{"meta":{"symbol":"USD/GBP","interval":"1day","currency_base":"US Dollar","currency_quote":"British Pound","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.78690","high":"0.78798","low":"0.78175","close":"0.78425"},{"datetime":"2023-08-03","open":"0.78680","high":"0.79212","low":"0.78569","close":"0.78685"},{"datetime":"2023-08-02","open":"0.78267","high":"0.78854","low":"0.78095","close":"0.78673"},{"datetime":"2023-08-01","open":"0.77911","high":"0.78479","low":"0.77870","close":"0.78269"}],"status":"ok"},"USD/CHF":{"meta":{"symbol":"USD/CHF","interval":"1day","currency_base":"US Dollar","currency_quote":"Swiss Franc","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.87425","high":"0.87840","low":"0.86990","close":"0.87290"},{"datetime":"2023-08-03","open":"0.87780","high":"0.87990","low":"0.87330","close":"0.87430"},{"datetime":"2023-08-02","open":"0.87525","high":"0.88060","low":"0.87160","close":"0.87755"},{"datetime":"2023-08-01","open":"0.87195","high":"0.87785","low":"0.87080","close":"0.87510"}],"status":"ok"}}'
        data = json.loads(data)
        return self.data_source.parse_data(data)

    def invert_currency_pair(self, pair: str):
        base, quote = pair.split('/')
        return f"{quote}/{base}"


class ForexService:
    def run(self):
        symbols = ["USD/CZK", "USD/EUR", "USD/GBP", "USD/CHF"]
        service = ForexScrapeService()
        api_data_source = ApiDataSource(None)
        service.set_data_source(api_data_source)
        data = service.get_data(symbols)

        symbol_models = {}
        for model in data:
            if model.symbol in symbol_models:
                symbol_models[model.symbol].append(model)
            else:
                symbol_models[model.symbol] = [model]

        forex_rate_analyzer = ForexRateAnalyzer()

        all_cross_rates = forex_rate_analyzer.find_all_cross_rates(symbol_models)
        print(all_cross_rates)
        symbol_models.update(all_cross_rates)

        # console log for test
        for key in symbol_models:
            print(f'{key}: [{symbol_models[key][-1].symbol}]')
            for price_record in symbol_models[key]:
                print(f'{price_record.symbol} {price_record.datetime} {price_record.close_price}')


forex_service = ForexService()
forex_service.run()