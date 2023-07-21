# importing datetime module
import datetime
import time
import requests
import json


class ResultData:
    def __init__(self, data):
        self.data = data


class Result:
    def __init__(self, timestamp, open_val, high_val, low_val, close_val, volume, quoteVolume):
        self.timestamp = timestamp
        self.open_val = open_val
        self.high_val = high_val
        self.low_val = low_val
        self.close_val = close_val
        self.volume = volume
        self.quoteVolume = quoteVolume


oneDayLimit = 86400
cryptoWatchBaseUrl = "https://api.cryptowat.ch"

date_time = datetime.datetime(2000, 7, 26, 21, 20)
fromTime = int(time.mktime(date_time.timetuple()))
url = f"{cryptoWatchBaseUrl}/markets/coinbase-pro/BTCUSD/ohlc?periods={oneDayLimit}&after={fromTime}"
print(url)

response = requests.get(url)
jsonData = response.text

parsed_data = json.loads(jsonData)

result_objects = [Result(*item) for item in parsed_data['result']['86400']]

result_data_instance = ResultData(result_objects)

for result in result_data_instance.data:
    print(f"Timestamp: {result.timestamp}, Close Value: {result.close_val}")
