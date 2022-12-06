import requests
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.common.exceptions import TimeoutException


class EquityActualPriceScraper:

    def get_stock_price(self):
        price = self.__donwload_price('AAPL')
        print(price[0])
        print(price[1])

    def __donwload_price(self, ticker):
        options = Options()
        options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
        options.headless = True
        driver = webdriver.Chrome(chrome_options=options,
                                  executable_path=r"D:\chromedriver_win32\chromedriver.exe", )
        driver.minimize_window()
        driver.get(f'https://www.google.com/search?q={ticker}+price')
        delay = 10

        try:
            _ = WebDriverWait(driver, delay, poll_frequency=7)
            page = driver.page_source
            print(page)
            driver.quit()
        except TimeoutException:
            driver.quit()

        soup = BeautifulSoup(page)

        priceHtml = soup.find('div', attrs={'data-attrid': 'Price'})
        spanMainPrice = priceHtml.findAll("span", recursive=False)[0]
        spanPrice = spanMainPrice.findAll("span", recursive=False)[0]
        price = spanPrice.findAll("span", recursive=False)[0]
        currency = spanPrice.findAll("span", recursive=False)[1]
        return price.text, currency.text


equity = EquityActualPriceScraper()
equity.get_stock_price()