from finnhub import Client


class FinnhubService:

    def __init__(self, finnhub_client: Client):
        self.__finnhub_client = finnhub_client

    def get_ticket_from_isin(self, isin: str) -> str | None:
        response = self.__finnhub_client.symbol_lookup(isin)

        if response.count == 0:
            return None

        return response.result[0]
