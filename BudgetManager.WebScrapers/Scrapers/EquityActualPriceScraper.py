import requests
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By
from selenium.common.exceptions import TimeoutException

request = requests.get(f'https://www.google.com/search?q=vusa+price')
soup = BeautifulSoup(request.text, "html.parser")
children = soup.find('div', attrs={'data-attrid': 'Price'})

print(soup)

options = Options()
options.binary_location = "C:\Program Files\Google\Chrome\Application\chrome.exe"
options.headless = True
driver = webdriver.Chrome(chrome_options=options, executable_path=r"D:\chromedriver_win32\chromedriver.exe", )
driver.minimize_window()
driver.get('https://www.google.com/search?q=vusa+price')

delay = 10
json_data_object = None

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
print(spanMainPrice)
print((price.text, currency.text))