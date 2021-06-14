import requests
from bs4 import BeautifulSoup
import datetime
from influxdb_client import InfluxDBClient, Point, WritePrecision
from influxdb_client.client.write_api import SYNCHRONOUS
import pytz

#DOWNLOAD M2 FROM CZ


class M2Model:
    def __init__(self, value, date):
        self.date = date
        self.value = value

    def save(self):
        bucket = "FinancialIndicators"
        client = InfluxDBClient(url="http://localhost:8086", token="14HjZx_tCQ0dksMwet66eSvKUmG9BBTjmlFMmWo28SvisishIf-A1FXQvwXvqdm09annuHX0iqrYZlOELK-Ciw==", org="8f46f33452affe4a")
        write_api = client.write_api(write_options=SYNCHRONOUS)
        p = Point("M2").field("value", float(self.value)).tag("state", "CZ").time(self.date.astimezone(pytz.utc), WritePrecision.NS)
        write_api.write(bucket=bucket, record=p)


page = requests.get("https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=57225&p_uka=8&p_strid=AAAG&p_od=200201&p_do=202104&p_lang=CS&p_format=0&p_decsep=%2C")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
m2Models = []

for tableRow in tableOfValues:
    values = tableRow.findAll('td')
    rawDate = str(values[0].text)
    value = str(values[1].text)
    index = rawDate.index("/")

    date = datetime.datetime(int(rawDate[index + 1:]), int(rawDate[:index]), 1)
    num = float(value.replace(",", "."))
    m2Models.append(M2Model(num, date))

for m2 in m2Models:
   m2.save()
   #  print(m2.value)