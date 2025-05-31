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
    """
    Service to manage stock tickers metadata by scraping data from TradingView
    and storing/updating it in the repository.
    """

    def __init__(self, stock_repo: StockRepository, tradingview_scraper: TradingviewScraper):
        """
        Initialize the service with a stock repository and a TradingView scraper.

        :param stock_repo: Repository interface for stock data.
        :param tradingview_scraper: Scraper service for TradingView data.
        """
        self.stock_repo = stock_repo
        self.tradingview_scraper = tradingview_scraper

    def check_tickers_metadata(self):
        """
        Checks all stock tickers metadata in the repository.
        If metadata is missing or incomplete, scrape fresh data and update it.
        Adds a 1-second delay between requests to avoid overloading the source.
        Logs errors but continues processing other tickers.
        """
        tickers = self.stock_repo.get_all_tickers_adjusted_info()

        for ticker in tickers:
            print(ticker._metadata)

            try:
                if ticker._metadata is None:
                    scraped_data = self.__get_scraped_metadata(ticker.code)
                    self.stock_repo.update_ticker_adjusted_metadata(ticker.code, dataclasses.asdict(scraped_data))
                else:
                    print(ticker._metadata, ticker.companyInfoTicker)
                    db_metadata = json.loads(ticker._metadata)
                    db_metadata_model = from_dict(TickerMetadata, db_metadata)

                    # Check if essential fields are missing
                    if db_metadata_model.isin is None or db_metadata_model.figi is None or db_metadata_model.currency is None:
                        scraped_data = self.__get_scraped_metadata(ticker.companyInfoTicker)
                        merged_data = {}

                        # Merge existing DB data
                        for key, value in db_metadata_model.__dict__.items():
                            merged_data[key] = value

                        # Merge scraped data, but keep existing price_ticker if ticker code differs
                        for key, value in scraped_data.__dict__.items():
                            if key == 'price_ticker' and ticker.companyInfoTicker != db_metadata_model.price_ticker:
                                continue
                            else:
                                merged_data[key] = value

                        print(merged_data)
                        self.stock_repo.update_ticker_adjusted_metadata(ticker.companyInfoTicker, merged_data)

                print(f'Metadata scraped for ticker {ticker.companyInfoTicker}')
                time.sleep(1)
            except Exception as e:
                logging.info('Error while downloading metadata for ticker: ' + ticker.code)
                logging.error(e)

    def __get_scraped_metadata(self, ticker: str):
        """
        Scrapes metadata for a given ticker using the TradingView scraper.

        :param ticker: Stock ticker symbol to scrape.
        :return: TickerMetadata object with scraped data.
        """
        isin, figi, symbol_info = self.tradingview_scraper.scrape_stock_data(ticker)
        return symbol_info
