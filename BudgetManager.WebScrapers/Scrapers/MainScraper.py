from Scrapers.Comodity.ComodityHistoryPriceScraper import ComodityPriceManager
from Scrapers.Crypto.CryptoHistoryPriceScraper import CryptoPriceManager
from Scrapers.Forex.ForexHistoryPriceScraper import ForexService
from Scrapers.Stocks.Final_StockPriceHistoryScraper import StockPriceManager

print('Scraping all data')

print('_________________________________________Stocks_________________________________________')
stock_manager = StockPriceManager()
stock_manager.scrape_tickers_price()

print('\n\n_________________________________________Crypto_________________________________________')
crypto_manager = CryptoPriceManager()
crypto_manager.scrape_crypto_price()

print('\n\n_________________________________________Comodity_________________________________________')
commodity_manager = ComodityPriceManager()
commodity_manager.scrape_comodity_price()

print('\n\n_________________________________________Forex_________________________________________')
forex_manager = ForexService()
forex_manager.run()
