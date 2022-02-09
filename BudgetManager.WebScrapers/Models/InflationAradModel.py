

class InflationAradModel:
    def __init__(self, date, customerPrice, netInflation, coreInflation, fuelInflation, monetaryRelevantInflation):
        self.date = date
        self.customerPrice = customerPrice
        self.netInflation = netInflation
        self.coreInflation = coreInflation
        self.fuelInflation = fuelInflation
        self.monetaryRelevantInflation = monetaryRelevantInflation
