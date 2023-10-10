from Services.YahooService import YahooService
from SourceFiles.stockList import stockToDownload

tickersToScrape = stockToDownload
yahooService = YahooService()

for ticker in tickersToScrape:
     split_data = yahooService.get_stock_split_history(ticker, '511056000', '1696896000')
     print(split_data)

