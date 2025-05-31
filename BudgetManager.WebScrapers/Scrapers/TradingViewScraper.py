import json
from dataclasses import dataclass
from typing import Optional, List, Any
import dacite
from bs4 import BeautifulSoup
import requests


@dataclass
class Source2:
    country: Optional[str] = None
    description: Optional[str] = None
    exchange_type: Optional[str] = None
    id: Optional[str] = None
    name: Optional[str] = None
    url: Optional[str] = None


@dataclass
class FinancialIndicator:
    financial_indicator_id: Optional[Any] = None
    base_currency_id: Optional[Any] = None
    timezone: Optional[str] = None
    is_etf: Optional[bool] = None
    is_crypto: Optional[bool] = None
    short_name: Optional[str] = None
    description: Optional[str] = None
    currency_id: Optional[str] = None
    currency: Optional[str] = None
    isin_displayed: Optional[str] = None
    has_fundamentals: Optional[bool] = None
    typespecs: Optional[List[str]] = None
    flag: Optional[str] = None
    country: Optional[str] = None
    local_description: Optional[str] = None
    root_description: Optional[str] = None
    resolved_symbol: Optional[str] = None
    primary_name: Optional[str] = None
    country_code_fund: Optional[str] = None
    underlying_symbol: Optional[str] = None
    exchange: Optional[str] = None
    is_derived_data: Optional[bool] = None
    pro_symbol: Optional[str] = None
    root: Optional[Any] = None
    source_logo_id: Optional[str] = None
    language: Optional[str] = None
    data_frequency: Optional[Any] = None
    source2: Optional[Source2] = None
    provider_id: Optional[str] = None
    has_price_snapshot: Optional[bool] = None
    is_spread: Optional[bool] = None
    type: Optional[str] = None
    short_description: Optional[str] = None
    currency_code: Optional[str] = None
    source_logo_url: Optional[str] = None
    exchange_for_display: Optional[str] = None
    product: Optional[Any] = None
    base_currency: Optional[Any] = None


@dataclass
class TickerMetadata:
    isin: Optional[str] = None
    figi: Optional[str] = None
    short_name: Optional[str] = None
    short_description: Optional[str] = None
    description: Optional[str] = None
    currency: Optional[str] = None
    resolved_symbol: Optional[str] = None
    exchange: Optional[str] = None
    pro_symbol: Optional[str] = None
    type: Optional[str] = None
    price_ticker: Optional[str] = None


class TradingviewScraper:

    def scrape_stock_data(self, ticker: str) -> (str | None, str | None, TickerMetadata):
        """
        Scrapes stock data from TradingView for a given ticker symbol.

        This method fetches comprehensive stock information including ISIN, FIGI,
        and various metadata fields by parsing the TradingView web page for the
        specified ticker symbol.

        Args:
            ticker (str): The stock ticker symbol to scrape data for (e.g., 'AAPL', 'MSFT').

        Returns:
            tuple: A tuple containing (isin, figi, mapped_data) where:
                - isin (str | None): The International Securities Identification Number
                - figi (str | None): The Financial Instrument Global Identifier
                - mapped_data (TickerMetadata): Object containing comprehensive stock metadata
                  including short_name, description, currency, exchange, etc.
                  Returns None for all values if scraping fails.
        """
        url = f"https://www.tradingview.com/symbols/{ticker}/"
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        }
        response = requests.get(url, headers=headers)

        if response.status_code == 200:
            soup = BeautifulSoup(response.text, 'html.parser')
            label_element = soup.find('div', string='FIGI')
            figi = None
            isin = None

            if label_element:
                # Navigate up to find the container, then find the value
                container = label_element.find_parent('div')
                while container:
                    # Look for a div that contains the FIGI pattern (BBG followed by alphanumeric)
                    value_divs = container.find_all('div')
                    for div in value_divs:
                        text = div.get_text(strip=True)
                        # FIGI pattern: starts with BBG followed by 9-12 alphanumeric characters
                        if text.startswith('BBG') and len(text) >= 12 and text[3:].isalnum():
                            figi = text
                    container = container.find_parent('div')

            label_element_isin = soup.find('div', string='ISIN')

            if label_element_isin:
                # Navigate up to find the container, then find the value
                container = label_element_isin.find_parent('div')
                while container:
                    # Look for a div that contains the ISIN pattern
                    value_divs = container.find_all('div')
                    for div in value_divs:
                        text = div.get_text(strip=True)
                        # ISIN pattern: 2 letters followed by 10 alphanumeric characters
                        if len(text) == 12 and text[:2].isalpha() and text[2:].isalnum():
                            isin = text
                    container = container.find_parent('div')

            script = soup.find('script', string=lambda s: s and 'window.initData.symbolInfo' in s)

            if script:
                # Extract JSON data from the script
                json_text = script.string.split('window.initData.symbolInfo = ')[1].strip().rstrip(';')
                data_dict = json.loads(json_text)
                symbol_info = dacite.from_dict(FinancialIndicator, data_dict)
                print(symbol_info)
                mappped_data = TickerMetadata(
                    isin=isin,
                    figi=figi,
                    short_name=symbol_info.short_name,
                    short_description=symbol_info.short_description,
                    description=symbol_info.description,
                    currency=symbol_info.currency,
                    resolved_symbol=symbol_info.resolved_symbol,
                    exchange=symbol_info.exchange,
                    pro_symbol=symbol_info.pro_symbol,
                    type=symbol_info.type
                    # price_ticker=ticker
                )
            return isin, figi, mappped_data
        else:
            print(f"Failed to retrieve the page. Status code: {response.status_code}")
            return None, None, None
