from Scrapers.Crypto.CryptoHistoryPriceScraper import CryptoTickers


class CryptoTickerTranslator:
    """
    Translates Kraken API ticker symbols to simplified ticker names.

    This is used to convert the complex Kraken ticker symbols (e.g., XXBTZUSD)
    to simpler, more readable symbols (e.g., BTC) for storage and display.
    """

    def translate(self, crypto_ticker: CryptoTickers) -> str:
        """
        Translate a Kraken ticker symbol to a simplified format.

        Args:
            crypto_ticker (CryptoTickers): The Kraken ticker enum value

        Returns:
            str: Simplified ticker symbol
        """
        match crypto_ticker:
            case CryptoTickers.XXBTZUSD:
                return "BTC"
            case CryptoTickers.XETHZUSD:
                return "ETH"
            case CryptoTickers.MATICUSD:
                return "MATIC"
            case CryptoTickers.LINKUSD:
                return "LINK"
            case CryptoTickers.SNXUSD:
                return "SNX"
            case CryptoTickers.USDCUSD:
                return "USDC"
