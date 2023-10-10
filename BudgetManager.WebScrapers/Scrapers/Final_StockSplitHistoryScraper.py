from Services.YahooService import YahooService, StockSplitData
from SourceFiles.stockList import stockToDownload
from datetime import datetime, timedelta
import logging


class StockSplitScraper:
    def scrape_stocks_splits(self, measurement: str, ticker: str, tag: str):
        try:
            stock_split_data: list[StockSplitData] = []
            date_to = datetime.now()
            # lastValue = self.influx_repo.filter_last_value(measurement, FilterTuple("ticker", tag), datetime.min)
            # TODO: replace influx to mssql table select
            lastValue = None

            # if len(lastValue) != 0:
            #     last_downloaded_time = lastValue[0].records[0]["_time"]
            #     now_datetime_with_offset = datetime.now().astimezone(last_downloaded_time.tzinfo) - timedelta(days=1)
            #
            #     if last_downloaded_time < now_datetime_with_offset:
            #         stock_split_data = self.__scrape_stock_data(ticker, last_downloaded_time, date_to)
            #         stock_split_data = [d for d in stock_split_data if d.date > last_downloaded_time]
            # else:
            #     stock_split_data = self.__scrape_stock_data(ticker, None, date_to)
            #
            # self.__save_price_data_to_influx(measurement, tag, stock_split_data)
        except Exception as e:
            logging.info('Error while downloading price for ticker: ' + ticker)
            logging.error(e)


tickersToScrape = stockToDownload
yahooService = YahooService()

for ticker in tickersToScrape:
    split_data = yahooService.get_stock_split_history(ticker, '511056000', '1696896000')
    print(split_data)
