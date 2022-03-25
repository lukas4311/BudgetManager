import finnhub
from secret import finnhubApiToken

finnhub_client = finnhub.Client(api_key=finnhubApiToken)

print(finnhub_client.company_profile2(symbol='AAPL'))