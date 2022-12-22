import selenium.webdriver
from selenium.webdriver.chrome.options import ChromiumOptions
from bs4 import BeautifulSoup
import time
import influxdb
from datetime import datetime

class GoogleStockScraper:
    def __init__(self, influx: 'InfluxService'):
        self.influx = influx

    def get_equity_price(self, ticker: str) -> float:
        options = ChromiumOptions()
        options.add_argument('--headless')
        driver = selenium.webdriver.Chrome(options=options)
        driver.get(f'https://www.google.com/search?q={ticker}+ticker')
        time.sleep(2)
        html = driver.page_source
        soup = BeautifulSoup(html, 'html.parser')
        results = soup.find_all('div', class_='g')
        for result in results:
            price_label_element = result.find('span', text=lambda x: x and 'Price:' in x)
            if price_label_element:
                price_element = price_label_element.next_sibling
                price_text = price_element.text
                price = float(price_text)
                break
        driver.close()
        return price

    def store_equity_price(self, ticker: str):
        self.influx.store_equity_price(ticker)

class InfluxService:
    def __init__(self, host: str, port: int, username: str, password: str, database: str):
        self.client = influxdb.InfluxDBClient(host=host, port=port, username=username, password=password)
        self.database = database

    def store_equity_price(self, ticker: str):
        self.client.switch_database(self.database)
        # Check if there is already data for the same ticker and date in the database
        query = f"from(bucket: \"{self.database}\") |> range(start: -1d) |> filter(fn: (r) => r._measurement == " \
                f"\"equity_prices\" and r.ticker == \"{ticker}\") |> count() "
        result = self.client.query(query)
        count = list(result.get_points())[0]['count']
        if count == 0:
            # There is no data, so store the current price in the database
            price = GoogleStockScraper(self).get_equity_price(ticker)
            data = [
                {
                    "measurement": "equity_prices",
                    "tags": {
                        "ticker": ticker
                    },
                    "fields": {
                        "price": price
                    },
                    "time": datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%SZ')
                }
            ]
            self.client.write_points(data)
