

class InflationAradModel:
    def __init__(self, date, customer_price, net_inflation, core_inflation, fuel_inflation, monetary_relevant_inflation):
        self.date = date
        self.customerPrice = customer_price
        self.netInflation = net_inflation
        self.coreInflation = core_inflation
        self.fuelInflation = fuel_inflation
        self.monetaryRelevantInflation = monetary_relevant_inflation
