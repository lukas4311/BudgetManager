import datetime
from dataclasses import dataclass

from influxdb_client import Point, WritePrecision
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By
from selenium.common.exceptions import TimeoutException
from bs4 import BeautifulSoup
import json
import pandas as pd
from Services.InfluxRepository import InfluxRepository
from configManager import token, organizaiton


@dataclass
class FinancialRecord:
    def __init__(self, description, financial_data_values):
        self.description = description
        self.financial_data_values = financial_data_values


@dataclass
class FinancialData:
    def __init__(self, date, value):
        self.date = date
        self.value = value


class MacroTrendScraper:
    url: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/income-statement?freq=A"
    pageSourceLocation = "var originalData = "
    influx_repository: InfluxRepository

    def __init__(self):
        self.influx_repository = InfluxRepository("http://localhost:8086", "Stocks", token, organizaiton)

    def download_income_statement(self, ticker: str):
        print(self.url.format(ticker=ticker))
        url_with_ticker = self.url.format(ticker=ticker)

        options = Options()
        options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        driver = webdriver.Chrome(chrome_options=options, executable_path=r"D:\chromedriver_win32\chromedriver.exe", )
        driver.minimize_window()
        driver.get(url_with_ticker)

        url = driver.current_url
        newurl = url + "?freq=A"
        driver.get(newurl)

        delay = 10  # seconds
        try:
            _ = WebDriverWait(driver, delay, poll_frequency=7).until(EC.presence_of_element_located((By.ID, 'columntablejqxgrid')))
            page = driver.page_source
            index = page.find(self.pageSourceLocation)
            index2 = page.rfind("var source") - 1
            substring = page[index + len(self.pageSourceLocation):index2]
            json_substring = substring[substring.find("["):substring.rfind("]") + 1]
            y = json.loads(json_substring)

            for jsonData in y:
                field_name = jsonData["field_name"]
                soup = BeautifulSoup(field_name, "html.parser")
                financial_data_values = []

                for val in jsonData:
                    if val != "field_name" and val != "popup_icon":
                        date = val
                        value = jsonData[val]
                        pandas_date = pd.to_datetime(date)
                        pandas_date = pandas_date.tz_localize("Europe/Prague")
                        pandas_date = pandas_date.tz_convert("utc")
                        financialData = FinancialData(pandas_date, value)
                        financial_data_values.append(financialData)

                financialRecord = FinancialRecord(soup.text, financial_data_values)
                self.save_financial_data(financialRecord, ticker)

            driver.quit()
        except TimeoutException:
            driver.quit()

    def save_financial_data(self, financial_record: FinancialRecord, ticker):
        measurement = "IncomeStatement"
        points = []
        fieldName = financial_record.description.replace('-', '').replace(' ', '')

        for data in financial_record.financial_data_values:
            if data.value != "":
                print(data.value)
                point = Point(measurement).time(data.date.utcnow(), WritePrecision.NS)
                point.field(fieldName, data.value)
                point.tag("ticker", ticker)
                points.append(point)

        if len(points) != 0:
            print(points)
        # self.influx_repository.add_range(points)
        # self.influx_repository.save()


test = MacroTrendScraper()
test.download_income_statement("AAPL")
