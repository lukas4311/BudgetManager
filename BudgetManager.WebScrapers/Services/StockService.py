import json

from dacite import from_dict

from Scrapers import TradingViewScraper
from Scrapers.TradingViewScraper import TickerMetadata
from Services.DB.StockRepository import StockRepository


class StockService:
    def __init__(self, stock_repo: StockRepository, tradingview_scraper: TradingViewScraper):
        self.stock_repo = stock_repo
        self.tradingview_scraper = tradingview_scraper

    def check_tickers_metadata(self):
        tickers = self.stock_repo.get_all_tickers('StockTradeTickers')

        for ticker in tickers:
            if ticker.metadata is not None:
                db_metadata = json.loads(ticker.metadata)
                db_metadata_model = from_dict(TickerMetadata, db_metadata)

                if db_metadata_model.isin is None or db_metadata_model.figi is None or db_metadata_model.currency is None:
                    merged_data = {}

                    # if obj2.key1 in merged_data:
                    #     merged_data[obj2.key1].key2 = obj2.key2
                    #     merged_data[obj2.key1].key3 = obj2.key3
                    # else:
                    #     merged_data[obj2.key1] = obj2


    def __get_scraped_metadata(self, ticker: str):
        isin, figi, symbol_info = self.tradingview_scraper.scrape_stock_data(ticker)
        return symbol_info
