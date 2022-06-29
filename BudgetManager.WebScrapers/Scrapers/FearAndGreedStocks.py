from dataclasses import dataclass
import requests
from bs4 import BeautifulSoup

from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime
from configManager import token
from configManager import organizaiton


from typing import List
from typing import Any
from dataclasses import dataclass


@dataclass
class Datum:
    x: float
    y: float
    rating: str

    @staticmethod
    def from_dict(obj: Any) -> 'Datum':
        _x = float(obj.get("x"))
        _y = float(obj.get("y"))
        _rating = str(obj.get("rating"))
        return Datum(_x, _y, _rating)


@dataclass
class FearAndGreed:
    score: float
    rating: str
    timestamp: str
    previous_close: float
    previous_1_week: float
    previous_1_month: float
    previous_1_year: float

    @staticmethod
    def from_dict(obj: Any) -> 'FearAndGreed':
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _timestamp = str(obj.get("timestamp"))
        _previous_close = float(obj.get("previous_close"))
        _previous_1_week = float(obj.get("previous_1_week"))
        _previous_1_month = float(obj.get("previous_1_month"))
        _previous_1_year = float(obj.get("previous_1_year"))
        return FearAndGreed(_score, _rating, _timestamp, _previous_close, _previous_1_week, _previous_1_month, _previous_1_year)


@dataclass
class FearAndGreedHistorical:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'FearAndGreedHistorical':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return FearAndGreedHistorical(_timestamp, _score, _rating, _data)


@dataclass
class JunkBondDemand:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'JunkBondDemand':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return JunkBondDemand(_timestamp, _score, _rating, _data)


@dataclass
class MarketMomentumSp125:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'MarketMomentumSp125':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return MarketMomentumSp125(_timestamp, _score, _rating, _data)


@dataclass
class MarketMomentumSp500:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'MarketMomentumSp500':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return MarketMomentumSp500(_timestamp, _score, _rating, _data)


@dataclass
class MarketVolatilityVix:
    timestamp: float
    score: int
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'MarketVolatilityVix':
        _timestamp = float(obj.get("timestamp"))
        _score = int(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return MarketVolatilityVix(_timestamp, _score, _rating, _data)


@dataclass
class MarketVolatilityVix50:
    timestamp: float
    score: int
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'MarketVolatilityVix50':
        _timestamp = float(obj.get("timestamp"))
        _score = int(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return MarketVolatilityVix50(_timestamp, _score, _rating, _data)


@dataclass
class PutCallOptions:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'PutCallOptions':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return PutCallOptions(_timestamp, _score, _rating, _data)


@dataclass
class StockPriceStrength:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'StockPriceStrength':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return StockPriceStrength(_timestamp, _score, _rating, _data)


@dataclass
class SafeHavenDemand:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'SafeHavenDemand':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return SafeHavenDemand(_timestamp, _score, _rating, _data)


@dataclass
class StockPriceBreadth:
    timestamp: float
    score: float
    rating: str
    data: List[Datum]

    @staticmethod
    def from_dict(obj: Any) -> 'StockPriceBreadth':
        _timestamp = float(obj.get("timestamp"))
        _score = float(obj.get("score"))
        _rating = str(obj.get("rating"))
        _data = [Datum.from_dict(y) for y in obj.get("data")]
        return StockPriceBreadth(_timestamp, _score, _rating, _data)


@dataclass
class Root:
    fear_and_greed: FearAndGreed
    fear_and_greed_historical: FearAndGreedHistorical
    market_momentum_sp500: MarketMomentumSp500
    market_momentum_sp125: MarketMomentumSp125
    stock_price_strength: StockPriceStrength
    stock_price_breadth: StockPriceBreadth
    put_call_options: PutCallOptions
    market_volatility_vix: MarketVolatilityVix
    market_volatility_vix_50: MarketVolatilityVix50
    junk_bond_demand: JunkBondDemand
    safe_haven_demand: SafeHavenDemand

    @staticmethod
    def from_dict(obj: Any) -> 'Root':
        _fear_and_greed = FearAndGreed.from_dict(obj.get("fear_and_greed"))
        _fear_and_greed_historical = FearAndGreedHistorical.from_dict(obj.get("fear_and_greed_historical"))
        _market_momentum_sp500 = MarketMomentumSp500.from_dict(obj.get("market_momentum_sp500"))
        _market_momentum_sp125 = MarketMomentumSp125.from_dict(obj.get("market_momentum_sp125"))
        _stock_price_strength = StockPriceStrength.from_dict(obj.get("stock_price_strength"))
        _stock_price_breadth = StockPriceBreadth.from_dict(obj.get("stock_price_breadth"))
        _put_call_options = PutCallOptions.from_dict(obj.get("put_call_options"))
        _market_volatility_vix = MarketVolatilityVix.from_dict(obj.get("market_volatility_vix"))
        _market_volatility_vix_50 = MarketVolatilityVix50.from_dict(obj.get("market_volatility_vix_50"))
        _junk_bond_demand = JunkBondDemand.from_dict(obj.get("junk_bond_demand"))
        _safe_haven_demand = SafeHavenDemand.from_dict(obj.get("safe_haven_demand"))
        return Root(_fear_and_greed, _fear_and_greed_historical, _market_momentum_sp500, _market_momentum_sp125, _stock_price_strength, _stock_price_breadth, _put_call_options, _market_volatility_vix, _market_volatility_vix_50, _junk_bond_demand, _safe_haven_demand)

# Example Usage
# jsonstring = json.loads(myjsonstring)
# root = Root.from_dict(jsonstring)


response = requests.get("https://production.dataviz.cnn.io/index/fearandgreed/graphdata")
# IT recognize bot test it through Chromium

jsonData = response.text
# soup = BeautifulSoup(page.content, 'html.parser')
#
# feerAndGreed = soup.findAll("span", {"class": "market-fng-gauge__dial-number-value"})[0]

print(jsonData)

    # allValues = list(needleChart.find_all('li'))
    # todayValue = str(allValues[0])
    #
    # searchedString = "Now: "
    # index = todayValue.find(searchedString)
    # startIndex = index + len(searchedString)
    #
    # fearAndGreedStocks = todayValue[startIndex:startIndex + 2]
    # print(fearAndGreedStocks)
    #
    # bucket = "StockFearAndGreed"
    # client = InfluxDBClient(url="http://localhost:8086", token=token, org=organizaiton)
    #
    # write_api = client.write_api(write_options=SYNCHRONOUS)
    # p = Point("fearAndGreed").field("value", float(fearAndGreedStocks)).time(datetime.utcnow(), WritePrecision.NS)
    # write_api.write(bucket=bucket, record=p)
