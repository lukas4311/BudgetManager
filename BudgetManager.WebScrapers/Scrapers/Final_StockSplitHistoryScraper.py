from Services.YahooService import YahooService

yahooService = YahooService()
split_data = yahooService.get_stock_split_history('AMZN', '511056000', '1696896000')

print(split_data)