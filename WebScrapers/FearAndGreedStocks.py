import requests
from bs4 import BeautifulSoup

page = requests.get("https://money.cnn.com/data/fear-and-greed/")
soup = BeautifulSoup(page.content, 'html.parser')

needleChart = soup.find_all(id="needleChart")[0];
allValues = list(needleChart.find_all('li'));
todayValue = str(allValues[0]);

searchedString = "Now: ";
index = todayValue.find(searchedString);
startIndex =index + len(searchedString)

fearAndGreedStocks = todayValue[startIndex:startIndex + 2];
print(fearAndGreedStocks);
