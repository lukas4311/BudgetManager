import dataclasses
import json
import logging
from datetime import datetime
import time
from dacite import from_dict

from Scrapers.TradingViewScraper import TradingviewScraper
from Scrapers.TradingViewScraper import TickerMetadata
from Services.DB.StockRepository import StockRepository

log_name = 'Logs/TickerMetadataScraping.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class StockService:
    def __init__(self, stock_repo: StockRepository, tradingview_scraper: TradingviewScraper):
        self.stock_repo = stock_repo
        self.tradingview_scraper = tradingview_scraper

    def check_tickers_metadata(self):
        tickers = self.stock_repo.get_all_tickers('StockTradeTickers')

        for ticker in tickers:
            try:
                if ticker._metadata is None:
                    scraped_data = self.__get_scraped_metadata(ticker.code)
                    self.stock_repo.update_ticker_metadata(ticker.code, 'StockTradeTickers',
                                                           dataclasses.asdict(scraped_data))
                else:
                    print(ticker._metadata, ticker.code)
                    db_metadata = json.loads(ticker._metadata)
                    db_metadata_model = from_dict(TickerMetadata, db_metadata)

                    if db_metadata_model.isin is None or db_metadata_model.figi is None or db_metadata_model.currency is None:
                        scraped_data = self.__get_scraped_metadata(ticker.code)
                        merged_data = {}

                        for key, value in db_metadata_model.__dict__.items():
                            if key not in merged_data:
                                merged_data[key] = value
                            else:
                                merged_data[key] = value

                        for key, value in scraped_data.__dict__.items():
                            if key not in merged_data:
                                merged_data[key] = value
                            else:
                                merged_data[key] = value

                        print(merged_data)
                        self.stock_repo.update_ticker_metadata(ticker.code, 'StockTradeTickers', merged_data)

                print(f'Metadata scraped for ticker {ticker.code}')
                time.sleep(1)
            except Exception as e:
                logging.info('Error while downloading metadata for ticker: ' + ticker.code)
                logging.error(e)

    def __get_scraped_metadata(self, ticker: str):
        isin, figi, symbol_info = self.tradingview_scraper.scrape_stock_data(ticker)
        return symbol_info
