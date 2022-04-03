from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By
from selenium.common.exceptions import TimeoutException
from bs4 import BeautifulSoup
import json

class MacroTrendScraper:
    url: str = "https://www.macrotrends.net/stocks/charts/{ticker}/{ticker}/income-statement?freq=A"

    def download_income_statement(self, ticker: str):
        print(self.url.format(ticker=ticker))
        url_with_ticker = self.url.format(ticker=ticker)

        options = Options()
        options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        driver = webdriver.Chrome(chrome_options=options, executable_path=r"D:\chromedriver_win32\chromedriver.exe", )
        driver.minimize_window()
        driver.get(url_with_ticker)

        url = driver.current_url
        newurl = url + "?freq=Q"
        driver.get(newurl)

        delay = 10  # seconds
        try:
            myElem = WebDriverWait(driver, delay, poll_frequency=7).until(
                EC.presence_of_element_located((By.ID, 'columntablejqxgrid')))
            print("Page is ready!")
            soup = BeautifulSoup(driver.page_source, "html.parser")
            # print(driver.page_source)
            page = driver.page_source
            index = page.find("var originalData = ")
            index2 = page.rfind("var source") - 1
            substring = page[index + len("var originalData = "):index2]
            json_substring = substring[substring.find("["):substring.rfind("]") + 1]
            # print(index)
            # print(index2)
            # print(substring)
            # print(json_substring)
            y = json.loads(json_substring)

            for jsonData in y:
                field_name = jsonData["field_name"]
                soup = BeautifulSoup(field_name, "html.parser")
                print(soup.text)
                for val in jsonData:
                    if val != "field_name" and val != "popup_icon":
                        date = val
                        value = jsonData[val]
                        print(date)
                        print(value)

            driver.quit()
        except TimeoutException:
            driver.quit()


test = MacroTrendScraper()
test.download_income_statement("AAPL")
