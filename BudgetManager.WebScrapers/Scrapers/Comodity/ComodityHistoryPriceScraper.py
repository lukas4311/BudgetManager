from datetime import datetime
from enum import Enum
import logging

from Scrapers.Final_StockPriceHistoryScraper import StockPriceScraper
from Services.InfluxRepository import InfluxRepository
from secret import influxDbUrl
from configManager import token, organizaiton

class ComodityTicker(Enum):
    GOLD = 'GC=F'
    SILVER = 'SI=F'
    OIL = 'CL=F'


log_name = 'ComodityHistoryPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s', level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "ComodityV2", token, organizaiton, logging)

tickersToScrape = ComodityTicker.GOLD.value
stockPriceScraper = StockPriceScraper(influx_repository)
stockPriceScraper.scrape_stocks_prices('Price', ComodityTicker.GOLD.value)