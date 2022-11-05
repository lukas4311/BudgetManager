import datetime
import logging

log_name = 'Logs/stockPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s', level=logging.DEBUG)

class StockPriceScraper:

    def scrapeStocksPrices(self):
        print("scraping")


stockPriceScraper = StockPriceScraper()
stockPriceScraper.scrapeStocksPrices()
