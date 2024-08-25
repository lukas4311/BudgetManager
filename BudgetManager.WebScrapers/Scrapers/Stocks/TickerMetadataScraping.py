from Scrapers.TradingViewScraper import TradingviewScraper
from Services.DB.StockRepository import StockRepository
from Services.StockService import StockService

stock_service = StockService(StockRepository(), TradingviewScraper())
stock_service.check_tickers_metadata()