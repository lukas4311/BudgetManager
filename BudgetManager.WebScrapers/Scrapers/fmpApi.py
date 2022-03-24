import requests

response = requests.get("https://financialmodelingprep.com/api/v3/profile/AAPL?apikey=")
print(response.json())