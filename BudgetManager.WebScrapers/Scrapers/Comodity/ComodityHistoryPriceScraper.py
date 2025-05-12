from datetime import datetime
from enum import Enum
import logging

from Scrapers.Stocks.Final_StockPriceHistoryScraper import StockPriceScraper
from Services.InfluxRepository import InfluxRepository
from secret import influxDbUrl
from secret import token, organizationId

log_name = 'ComodityHistoryPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "ComodityV2", token, organizationId, logging)


class ComodityTicker(Enum):
    Gold = 'GC=F'
    Silver = 'SI=F'
    Oil = 'CL=F'


class ComodityPriceManager:
    def scrape_comodity_price(self):
        stock_price_scraper = StockPriceScraper(influx_repository)
        stock_price_scraper.scrape_stocks_prices('Price', ComodityTicker.Gold.value, ComodityTicker.Gold.name)
        stock_price_scraper.scrape_stocks_prices('Price', ComodityTicker.Silver.value, ComodityTicker.Silver.name)
        stock_price_scraper.scrape_stocks_prices('Price', ComodityTicker.Oil.value, ComodityTicker.Oil.name)
