import json
import logging
from dataclasses import dataclass
from datetime import datetime, timezone

import pandas as pd
import requests
from influxdb_client import Point, WritePrecision

from Models.FilterTuple import FilterTuple
from Services.InfluxRepository import InfluxRepository
from secret import token, organizationId
from secret import influxDbUrl
from secret import tokenTwelveData

log_name = 'Logs/forexPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "ForexV2", token, organizationId, logging)
measurement = "ExchangeRates"


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
        return f"{base_url}?symbol={symbol_param}&interval={interval}&apikey={api_key}&start_date=2000-01-01"


class ApiDataSource:
    def __init__(self, api_url):
        self.api_url = api_url

    def fetch_data(self):
        response = requests.get(self.api_url)

        if response.status_code != 200:
            raise ValueError("Failed to fetch data from the API.")

        return response.json()

    def parse_data(self, data) -> PriceModel:
        parsed_data = []
        for symbol, symbol_data in data.items():
            if 'values' not in symbol_data:
                continue

            for entry in symbol_data['values']:
                datetime_str = entry['datetime']
                close_price = entry['close']
                datetime = pd.to_datetime(datetime_str, utc=True)
                model = PriceModel(datetime=datetime, close_price=round(float(close_price), 6), symbol=symbol)
                parsed_data.append(model)

        return parsed_data


class ForexRateAnalyzer:
    def find_all_cross_rates(self, price_data: dict) -> dict:
        cross_data = {}

        for first_symbol, first_symbol_exchange_rates in price_data.items():
            for second_symbol, second_symbol_exchange_rates in price_data.items():
                if first_symbol == second_symbol or second_symbol + "/" + first_symbol in price_data:
                    continue
                cross_key = first_symbol + "/" + second_symbol
                cross_value = list(zip(first_symbol_exchange_rates, second_symbol_exchange_rates))

                for v1, v2 in cross_value:
                    if v1.datetime == v2.datetime:
                        cross_rate = round(v1.close_price / v2.close_price, 6)
                        cross_model = PriceModel(v1.datetime, cross_rate, cross_key)
                        base_currency = cross_key.split("/")[1]
                        quote_currency = cross_key.split("/")[-1]
                        key = quote_currency + "/" + base_currency
                        cross_model.symbol = key
                        if key in cross_data:
                            cross_data[key].append(cross_model)
                        else:
                            cross_data[key] = [cross_model]

        return cross_data

    def get_reversed_data(self, price_data: dict):
        reverse_data = {}

        for symbol, entries in price_data.items():
            reversed_symbol = self.__reverse_symbol(symbol)
            reversed_entries = [PriceModel(entry.datetime, 1 / entry.close_price, reversed_symbol) for entry in entries]
            reverse_data[reversed_symbol] = reversed_entries

        return reverse_data

    def __reverse_symbol(self, symbol):
        return f"{symbol.split('/')[1]}/{symbol.split('/')[0]}"


class ForexScrapeService:
    def __init__(self):
        self.data_source = None

    def set_data_source(self, data_source):
        self.data_source = data_source

    def get_data(self, symbols) -> list[PriceModel]:
        if not self.data_source:
            raise ValueError("Data source is not set. Please call set_data_source() first.")

        api_url = TwelveDataUrlBuilder.build_time_series_url(symbols)
        self.data_source.api_url = api_url
        json_data = self.data_source.fetch_data()

        # with open('forexData.json', "w") as json_file:
        #     json.dump(json_data, json_file)

        # with open('forexData.json', "r") as json_file:
        #     json_data = json.load(json_file)

        # data = '{"USD/CZK":{"meta":{"symbol":"USD/CZK","interval":"1day","currency_base":"US Dollar","currency_quote":"Czech Koruna","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"22.11000","high":"22.21170","low":"21.94720","close":"22.03530"},{"datetime":"2023-08-03","open":"21.91590","high":"22.21390","low":"21.87700","close":"22.13430"},{"datetime":"2023-08-02","open":"21.80990","high":"21.97790","low":"21.71790","close":"21.91560"},{"datetime":"2023-08-01","open":"21.72250","high":"21.86480","low":"21.69340","close":"21.81840"}],"status":"ok"},"USD/EUR":{"meta":{"symbol":"USD/EUR","interval":"1day","currency_base":"US Dollar","currency_quote":"Euro","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.91334","high":"0.91448","low":"0.90568","close":"0.90820"},{"datetime":"2023-08-03","open":"0.91424","high":"0.91629","low":"0.91229","close":"0.91331"},{"datetime":"2023-08-02","open":"0.91043","high":"0.91578","low":"0.90746","close":"0.91420"},{"datetime":"2023-08-01","open":"0.90930","high":"0.91302","low":"0.90870","close":"0.91035"}],"status":"ok"},"USD/GBP":{"meta":{"symbol":"USD/GBP","interval":"1day","currency_base":"US Dollar","currency_quote":"British Pound","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.78690","high":"0.78798","low":"0.78175","close":"0.78425"},{"datetime":"2023-08-03","open":"0.78680","high":"0.79212","low":"0.78569","close":"0.78685"},{"datetime":"2023-08-02","open":"0.78267","high":"0.78854","low":"0.78095","close":"0.78673"},{"datetime":"2023-08-01","open":"0.77911","high":"0.78479","low":"0.77870","close":"0.78269"}],"status":"ok"},"USD/CHF":{"meta":{"symbol":"USD/CHF","interval":"1day","currency_base":"US Dollar","currency_quote":"Swiss Franc","type":"Physical Currency"},"values":[{"datetime":"2023-08-04","open":"0.87425","high":"0.87840","low":"0.86990","close":"0.87290"},{"datetime":"2023-08-03","open":"0.87780","high":"0.87990","low":"0.87330","close":"0.87430"},{"datetime":"2023-08-02","open":"0.87525","high":"0.88060","low":"0.87160","close":"0.87755"},{"datetime":"2023-08-01","open":"0.87195","high":"0.87785","low":"0.87080","close":"0.87510"}],"status":"ok"}}'
        # json_data = json.loads(json_data)
        return self.data_source.parse_data(json_data)



class ForexService:
    def run(self):
        symbols = ["USD/CZK", "USD/EUR", "USD/GBP", "USD/CHF", "USD/JPY"]
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
        inverse_rates = forex_rate_analyzer.get_reversed_data(symbol_models)
        all_cross_rates = forex_rate_analyzer.find_all_cross_rates(symbol_models)

        symbol_models.update(inverse_rates)
        symbol_models.update(all_cross_rates)

        # console log for test
        for key in symbol_models:
            print(f'{key}: [{symbol_models[key][-1].symbol}]')

            exchangeRates = symbol_models[key]
            last_record = self.get_last_record_time(key)
            print(f"last record: {last_record}")
            filteredExchangeRates = [d for d in exchangeRates if
                                     datetime.now().astimezone(d.datetime.tzinfo) > d.datetime > last_record]

            if len(filteredExchangeRates) > 0:
                self.save_data_to_influx(filteredExchangeRates)
            else:
                print(f"No new data for pair: {key}")

    def save_data_to_influx(self, priceData: list[PriceModel]):
        pointsToSave = []
        transferred_symbol = priceData[0].symbol.replace('/', '-')
        logging.info('Saving forex pair: ' + transferred_symbol)

        for priceModel in priceData:
            point = Point(measurement) \
                .tag("pair", priceModel.symbol.replace('/', '-')) \
                .field('price', priceModel.close_price)
            point = point.time(priceModel.datetime, WritePrecision.NS)
            pointsToSave.append(point)

        influx_repository.add_range(pointsToSave)
        for point in pointsToSave:
            print(point.to_line_protocol())

        influx_repository.save()
        logging.info('Data saved for pair: ' + priceData[0].symbol)
        print("Data saved")

    def get_last_record_time(self, ticker: str):
        transferred_symbol = ticker.replace('/', '-')
        lastValue = influx_repository.filter_last_value(measurement, FilterTuple("pair", transferred_symbol), datetime.min)
        last_downloaded_time = datetime(1975, 1, 1, 0, 0, 0, tzinfo=timezone.utc)

        if len(lastValue) != 0:
            last_downloaded_time = lastValue[0].records[0]["_time"]

        return last_downloaded_time