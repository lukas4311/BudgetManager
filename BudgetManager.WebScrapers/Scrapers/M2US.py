from Services.InfluxRepository import InfluxRepository
from Services.MoneySupplyUsService import MoneySupplyUsService
from config import token, organizaiton

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, organizaiton)
moneySupplyService = MoneySupplyUsService(influx_repository)
moneySupplyService.download_us_money_supply_data("https://fred.stlouisfed.org/data/M2SL.txt", "M2")