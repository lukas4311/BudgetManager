from Services.InfluxRepository import InfluxRepository
from Services.MoneySupplyUsService import MoneySupplyUsService
from configManager import token

influx_repository = InfluxRepository("http://localhost:8086", "FinancialIndicators", token, "8f46f33452affe4a")
moneySupplyService = MoneySupplyUsService(influx_repository)
moneySupplyService.download_us_money_supply_data("https://fred.stlouisfed.org/data/M1NS.txt", "M1")