import json
from dataclasses import dataclass, asdict
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


class TradingviewScraper:

    def scrape_data(self, ticker:str):
        url = f"https://www.tradingview.com/symbols/{ticker}/"
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        }
        response = requests.get(url, headers=headers)

        if response.status_code == 200:
            soup = BeautifulSoup(response.text, 'html.parser')

            def find_value(label):
                label_elem = soup.find('div', class_='label-GgmpMpKr', string=label)
                if label_elem:
                    block = label_elem.find_parent('div', class_='block-GgmpMpKr')
                    if block:
                        value_elem = block.find('div', class_='value-GgmpMpKr')
                        if value_elem:
                            return value_elem.text.strip()
                return None

            # Find ISIN and FIGI
            isin = find_value('ISIN')
            figi = find_value('FIGI')

            # Find Currency
            # Find the script tag containing the JSON data
            script = soup.find('script', string=lambda s: s and 'window.initData.symbolInfo' in s)

            if script:
                # Extract JSON data from the script
                json_text = script.string.split('window.initData.symbolInfo = ')[1].strip().rstrip(';')
                print(json_text)
                data_dict = json.loads(json_text)
                symbol_info = dacite.from_dict(FinancialIndicator, data_dict)

            return isin, figi, symbol_info
        else:
            print(f"Failed to retrieve the page. Status code: {response.status_code}")
            return None, None, None


tradingview_scraper = TradingviewScraper()

# Call the function
isin, figi, symbol_info = tradingview_scraper.scrape_data('VUAA')

metadata = TickerMetadata(
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
)

metadata = json.dumps(asdict(metadata))
print(metadata)
