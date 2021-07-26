import requests
from bs4 import BeautifulSoup
from Services.InfluxRepository import InfluxRepository
from Services.MoneySupplyCz import MoneySupplyCz
from configManager import token

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
page = requests.get(
    "https://www.cnb.cz/cnb/STAT.ARADY_PKG.VYSTUP?p_period=1&p_sort=2&p_des=50&p_sestuid=57225&p_uka=8&p_strid=AAAG&p_od=200201&p_do=202104&p_lang=CS&p_format=0&p_decsep=%2C")
soup = BeautifulSoup(page.content, 'html.parser')
tableOfValues = soup.findChild("tbody").findAll('tr')
moneySupplyService = MoneySupplyCz(influx_repository)
moneySupplyService.download_money_supply_data("M2", tableOfValues)
