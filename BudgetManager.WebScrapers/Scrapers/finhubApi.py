import finnhub
from configManager import token, organizaiton

finnhub_client = finnhub.Client(api_key="")

print(finnhub_client.company_profile2(symbol='AAPL'))