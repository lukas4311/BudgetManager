import pytz
from influxdb_client import Point, WritePrecision
import pandas as pd
from Services.InfluxRepository import InfluxRepository
from Services.RoicService import RoicService
from configManager import token, organizaiton
from secret import influxDbUrl

roic_service = RoicService()
influx_repository = InfluxRepository(influxDbUrl, "StocksRoic", token, organizaiton)

ticker = 'AAPL'
data = roic_service.get_fin_summary(ticker)
print(data)

for pointData in data:
    if(pointData.year != 'TTM'):
        pandas_date = pd.to_datetime(f"{pointData.year}-01-01")
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        date = pandas_date.astimezone(pytz.utc)
        point = Point('FinSummary').time(date, WritePrecision.NS).tag("ticker", ticker).field(pointData.name, pointData.value)
        print(point)

# influx_repository.add_range(points)
# influx_repository.save()
# points = []