from datetime import datetime
import logging
from secret import alphaVantageToken

from Services.AlphaVantage import AlphaVantageService

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class StockPriceScraper:

    def scrapeStocksPrices(self):
        print("scraping")
        alphaVantageService = AlphaVantageService(alphaVantageToken)
        alphaVantageService.get_stock_price_history('AAPL')


stockPriceScraper = StockPriceScraper()
stockPriceScraper.scrapeStocksPrices()
