import datetime
import pytz
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
from config import token, organizaiton
from config import influxUrl
utc = pytz.UTC


@dataclass
class FinancialData:
    def __init__(self, date: datetime.datetime, value: float):
        self.date = date
        self.value = value


@dataclass
class FinancialRecord:
    def __init__(self, description: str, financial_data_values:  list[FinancialData]):
        self.description = description
        self.financial_data_values = financial_data_values


class MacroTrendScraper:
    __url_income_statement: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/income-statement?freq=A"
    __url_balance_sheet: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/balance-sheet?freq=A"
    __url_financial_ratios: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/financial-ratios?freq=A"
    __url_cash_flow_statement: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/cash-flow-statement?freq=A"

    pageSourceLocation = "var originalData = "
    mainJsGridId = "columntablejqxgrid"
    influx_repository: InfluxRepository
    points = []

    def __init__(self):
        self.influx_repository = InfluxRepository(influxUrl, "Stocks", token, organizaiton)

    def download_income_statement(self, ticker: str, frequency: str = "A"):
        self.__download_data(self.__url_income_statement, ticker, frequency, "IncomeStatement", None)

    def download_income_statement_from_date(self, ticker: str, from_date: datetime, frequency: str = "A"):
        self.__download_data(self.__url_income_statement, ticker, frequency, "IncomeStatement", from_date)

    def download_balance_sheet(self, ticker: str, frequency: str = "A"):
        self.__download_data(self.__url_balance_sheet, ticker, frequency, "BalanceSheet", None)

    def download_balance_sheet_from_date(self, ticker: str, from_date: datetime, frequency: str = "A"):
        self.__download_data(self.__url_income_statement, ticker, frequency, "BalanceSheet", from_date)

    def download_cash_flow(self, ticker: str, frequency: str = "A"):
        self.__download_data(self.__url_cash_flow_statement, ticker, frequency, "CashFlow", None)

    def download_cash_flow_from_date(self, ticker: str, from_date: datetime, frequency: str = "A"):
        self.__download_data(self.__url_cash_flow_statement, ticker, frequency, "CashFlow", from_date)

    def __download_data(self, url: str, ticker: str, frequency: str, measurement: str, from_date: datetime):
        print(url.format(ticker=ticker))
        url_with_ticker = url.format(ticker=ticker)

        options = Options()
        options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        options.headless = True
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

        if json_data_object is not None:
            for jsonData in json_data_object:
                field_name = jsonData["field_name"]
                parsed_filed_element = BeautifulSoup(field_name, "html.parser")
                financial_data_values = []

                for val in jsonData:
                    if val != "field_name" and val != "popup_icon":
                        date = val
                        value = jsonData[val]
                        parsed_date = self.__parse_date_to_pandas_date(date)
                        from_date = utc.localize(from_date) if from_date is not None and from_date.tzinfo is None else from_date

                        if from_date is None or from_date < parsed_date:
                            financial_data = FinancialData(parsed_date, value)
                            financial_data_values.append(financial_data)

                financial_record = FinancialRecord(parsed_filed_element.text, financial_data_values)
                self.__save_data(financial_record, ticker, measurement, frequency)

            self.influx_repository.add_range(self.points)
            self.influx_repository.save()
            self.points = []


    def __parse_date_to_pandas_date(self, dateString: str):
        pandas_date = pd.to_datetime(dateString)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        return pandas_date.astimezone(pytz.utc)

    def __save_data(self, financial_record: FinancialRecord, ticker, measurement, frequency):
        field_name = financial_record.description.replace('-', '').replace(' ', '')

        for data in financial_record.financial_data_values:
            if data.value != "":
                calculated_value = float(data.value) * 1000000
                point = Point(measurement).time(data.date, WritePrecision.NS).tag("ticker", ticker)\
                    .tag("frequency", frequency).field(field_name, calculated_value)
                print(f'Data saved ({ticker}): ' + point.to_line_protocol())
                self.points.append(point)


# Testing ...
# test = MacroTrendScraper()
# test.download_income_statement_from_date("AMZN", datetime.datetime(2021, 12, 31))
# test.download_income_statement("AMZN")
