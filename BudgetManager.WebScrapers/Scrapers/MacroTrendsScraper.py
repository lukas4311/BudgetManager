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
from config import token, organizaiton, macro_trends_url
from config import influxUrl

utc = pytz.UTC


@dataclass
class FinancialData:
    """
    Represents a single financial data point with a date and value.

    Attributes:
        date (datetime.datetime): The date associated with the financial data
        value (float): The numerical value of the financial metric
    """

    def __init__(self, date: datetime.datetime, value: float):
        """
        Initialize a FinancialData instance.

        Args:
            date (datetime.datetime): The date for this financial data point
            value (float): The monetary or ratio value for this data point
        """
        self.date = date
        self.value = value


@dataclass
class FinancialRecord:
    """
    Represents a collection of financial data points for a specific metric.

    Contains a description of the financial metric and all associated
    time-series data points for that metric.

    Attributes:
        description (str): Human-readable description of the financial metric
        financial_data_values (list[FinancialData]): Time-series data points
    """

    def __init__(self, description: str, financial_data_values: list[FinancialData]):
        """
        Initialize a FinancialRecord instance.

        Args:
            description (str): Description of the financial metric (e.g., "Revenue", "Net Income")
            financial_data_values (list[FinancialData]): List of data points across time periods
        """
        self.description = description
        self.financial_data_values = financial_data_values


class MacroTrendScraper:
    """
    Web scraper for financial data from MacroTrends.net.

    This scraper extracts financial statements and ratios using Selenium WebDriver
    to handle JavaScript-rendered content. It supports income statements, balance sheets,
    cash flow statements, and financial ratios with both annual and quarterly frequencies.

    The scraped data is automatically stored in InfluxDB for time-series analysis.
    """

    __url_income_statement: str = "{macro_base_url}/stocks/charts/{ticker}/{ticker}/income-statement?freq=A"
    __url_balance_sheet: str = "{macro_base_url}/stocks/charts/{ticker}/{ticker}/balance-sheet?freq=A"
    __url_financial_ratios: str = "{macro_base_url}/stocks/charts/{ticker}/{ticker}/financial-ratios?freq=A"
    __url_cash_flow_statement: str = "{macro_base_url}/stocks/charts/{ticker}/{ticker}/cash-flow-statement?freq=A"

    pageSourceLocation = "var originalData = "
    mainJsGridId = "columntablejqxgrid"
    influx_repository: InfluxRepository
    points = []

    def __init__(self):
        """
        Initialize the MacroTrendScraper with InfluxDB repository connection.

        Sets up the InfluxDB repository using configuration from the config module
        to store scraped financial data in the "Stocks" bucket.
        """
        self.influx_repository = InfluxRepository(influxUrl, "Stocks", token, organizaiton)

    def download_income_statement(self, ticker: str, frequency: str = "A") -> None:
        """
        Downloads complete income statement data for a given ticker.

        Scrapes all available income statement data (revenue, expenses, net income, etc.)
        from MacroTrends and stores it in InfluxDB.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_income_statement, ticker, frequency, "IncomeStatement", None)

    def download_income_statement_from_date(self, ticker: str, from_date: datetime, frequency: str = "A") -> None:
        """
        Downloads income statement data for a ticker from a specific date onwards.

        Only retrieves and stores income statement data that is newer than the specified
        from_date, useful for incremental updates.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            from_date (datetime): Only download data newer than this date
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_income_statement, ticker, frequency, "IncomeStatement", from_date)

    def download_balance_sheet(self, ticker: str, frequency: str = "A") -> None:
        """
        Downloads complete balance sheet data for a given ticker.

        Scrapes all available balance sheet data (assets, liabilities, equity, etc.)
        from MacroTrends and stores it in InfluxDB.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_balance_sheet, ticker, frequency, "BalanceSheet", None)

    def download_balance_sheet_from_date(self, ticker: str, from_date: datetime, frequency: str = "A") -> None:
        """
        Downloads balance sheet data for a ticker from a specific date onwards.

        Only retrieves and stores balance sheet data that is newer than the specified
        from_date, useful for incremental updates.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            from_date (datetime): Only download data newer than this date
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_income_statement, ticker, frequency, "BalanceSheet", from_date)

    def download_cash_flow(self, ticker: str, frequency: str = "A") -> None:
        """
        Downloads complete cash flow statement data for a given ticker.

        Scrapes all available cash flow data (operating, investing, financing activities)
        from MacroTrends and stores it in InfluxDB.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_cash_flow_statement, ticker, frequency, "CashFlow", None)

    def download_cash_flow_from_date(self, ticker: str, from_date: datetime, frequency: str = "A") -> None:
        """
        Downloads cash flow statement data for a ticker from a specific date onwards.

        Only retrieves and stores cash flow data that is newer than the specified
        from_date, useful for incremental updates.

        Args:
            ticker (str): Stock ticker symbol (e.g., 'AAPL', 'MSFT')
            from_date (datetime): Only download data newer than this date
            frequency (str, optional): Data frequency - 'A' for annual, 'Q' for quarterly.
                                     Defaults to 'A'.
        """
        self.__download_data(self.__url_cash_flow_statement, ticker, frequency, "CashFlow", from_date)

    def __download_data(self, url: str, ticker: str, frequency: str, measurement: str, from_date: datetime) -> None:
        """
        Internal method to handle the web scraping and data processing logic.

        Uses Selenium WebDriver to navigate to MacroTrends, wait for JavaScript content
        to load, extract JSON data from the page source, and process financial records.

        Args:
            url (str): URL template for the specific financial statement type
            ticker (str): Stock ticker symbol
            frequency (str): Data frequency ('A' for annual, 'Q' for quarterly)
            measurement (str): InfluxDB measurement name for categorizing data
            from_date (datetime): Optional filter to only process data after this date
        """
        print(url.format(ticker=ticker))
        url_with_ticker = url.format(macro_base_url = macro_trends_url,ticker=ticker)

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
            _ = WebDriverWait(driver, delay, poll_frequency=7).until(
                EC.presence_of_element_located((By.ID, self.mainJsGridId)))
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
                        from_date = utc.localize(
                            from_date) if from_date is not None and from_date.tzinfo is None else from_date

                        if from_date is None or from_date < parsed_date:
                            financial_data = FinancialData(parsed_date, value)
                            financial_data_values.append(financial_data)

                financial_record = FinancialRecord(parsed_filed_element.text, financial_data_values)
                self.__save_data(financial_record, ticker, measurement, frequency)

            self.influx_repository.add_range(self.points)
            self.influx_repository.save()
            self.points = []

    def __parse_date_to_pandas_date(self, dateString: str) -> datetime:
        """
        Converts a date string to a timezone-aware datetime object.

        Parses the input date string using pandas, localizes it to Prague timezone,
        then converts it to UTC for consistent storage.

        Args:
            dateString (str): Date string in a format parseable by pandas
                            (e.g., "2023-12-31", "Dec 31, 2023")

        Returns:
            datetime: UTC timezone-aware datetime object
        """
        pandas_date = pd.to_datetime(dateString)
        pandas_date = pandas_date.tz_localize("Europe/Prague")
        pandas_date.tz_convert("utc")
        return pandas_date.astimezone(pytz.utc)

    def __save_data(self, financial_record: FinancialRecord, ticker: str, measurement: str, frequency: str) -> None:
        """
        Processes a financial record and prepares InfluxDB points for storage.

        Converts financial record data into InfluxDB Point objects with proper
        tags and fields. Values are multiplied by 1,000,000 to convert from
        millions to actual amounts.

        Args:
            financial_record (FinancialRecord): The financial record to process
            ticker (str): Stock ticker symbol for tagging
            measurement (str): InfluxDB measurement name
            frequency (str): Data frequency for tagging ('A' or 'Q')
        """
        field_name = financial_record.description.replace('-', '').replace(' ', '')

        for data in financial_record.financial_data_values:
            if data.value != "":
                calculated_value = float(data.value) * 1000000
                point = Point(measurement).time(data.date, WritePrecision.NS).tag("ticker", ticker) \
                    .tag("frequency", frequency).field(field_name, calculated_value)
                print(f'Data saved ({ticker}): ' + point.to_line_protocol())
                self.points.append(point)

# Testing ...
# test = MacroTrendScraper()
# test.download_income_statement_from_date("AMZN", datetime.datetime(2021, 12, 31))
# test.download_income_statement("AMZN")