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
    url_income_statement: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/income-statement?freq=A"
    url_balance_sheet: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/balance-sheet?freq=A"
    url_financial_ratios: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/financial-ratios?freq=A"
    url_cash_flow_statement: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/cash-flow-statement?freq=A"

    pageSourceLocation = "var originalData = "
    mainJsGridId = "columntablejqxgrid"
    influx_repository: InfluxRepository

    def __init__(self):
        self.influx_repository = InfluxRepository("http://localhost:8086", "Stocks", token, organizaiton)

    def download_income_statement(self, ticker: str, frequency: str = "A"):
        print(self.url_balance_sheet.format(ticker=ticker))
        url_with_ticker = self.url_balance_sheet.format(ticker=ticker)

        options = Options()
        options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        driver = webdriver.Chrome(chrome_options=options, executable_path=r"D:\chromedriver_win32\chromedriver.exe", )
        driver.minimize_window()
        driver.get(url_with_ticker)

        url = driver.current_url
        url_with_frequency = url + "?freq=" + frequency
        driver.get(url_with_frequency)
        delay = 10
        json_data_object = None

        try:
            _ = WebDriverWait(driver, delay, poll_frequency=7).until(EC.presence_of_element_located((By.ID, self.mainJsGridId)))
            page = driver.page_source
            index = page.find(self.pageSourceLocation)
            index2 = page.rfind("var source") - 1
            substring = page[index + len(self.pageSourceLocation):index2]
            json_substring = substring[substring.find("["):substring.rfind("]") + 1]
            json_data_object = json.loads(json_substring)
            driver.quit()
        except TimeoutException:
            driver.quit()

        for jsonData in json_data_object:
            field_name = jsonData["field_name"]
            parsed_filed_element = BeautifulSoup(field_name, "html.parser")
            financial_data_values = []

            for val in jsonData:
                if val != "field_name" and val != "popup_icon":
                    date = val
                    value = jsonData[val]
                    pandas_date = self.parse_date_to_pandas_date(date)
                    financialData = FinancialData(pandas_date, value)
                    financial_data_values.append(financialData)

            financialRecord = FinancialRecord(parsed_filed_element.text, financial_data_values)
            self.save_data(financialRecord, ticker)

    def parse_date_to_pandas_date(self, dateString: str):
        pandas_date = pd.to_datetime(dateString)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        return pandas_date.tz_convert("utc")

    def save_data(self, financial_record: FinancialRecord, ticker):
        measurement = "IncomeStatement"
        points = []
        fieldName = financial_record.description.replace('-', '').replace(' ', '')
        print(fieldName)

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
