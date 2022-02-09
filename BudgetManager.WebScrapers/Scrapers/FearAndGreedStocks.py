import requests
from bs4 import BeautifulSoup

from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime
from configManager import token
from configManager import organizaiton

page = requests.get("https://money.cnn.com/data/fear-and-greed/")
soup = BeautifulSoup(page.content, 'html.parser')

needleChart = soup.find_all(id="needleChart")[0]
allValues = list(needleChart.find_all('li'))
todayValue = str(allValues[0])

searchedString = "Now: "
index = todayValue.find(searchedString)
startIndex = index + len(searchedString)

fearAndGreedStocks = todayValue[startIndex:startIndex + 2]
print(fearAndGreedStocks)

bucket = "StockFearAndGreed"
client = InfluxDBClient(url="http://localhost:8086", token=token, org=organizaiton)

write_api = client.write_api(write_options=SYNCHRONOUS)
p = Point("fearAndGreed").field("value", float(fearAndGreedStocks)).time(datetime.utcnow(), WritePrecision.NS)
write_api.write(bucket=bucket, record=p)
