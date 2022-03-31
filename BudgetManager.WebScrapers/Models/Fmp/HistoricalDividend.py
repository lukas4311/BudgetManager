from dataclasses import dataclass


@dataclass
class HistoricalDividend:
    date: str
    label: str
    adjDividend: float
    dividend: float
    recordDate: str
    paymentDate: str
    declarationDate: str

    @staticmethod
    def create_from_json(data):
        return HistoricalDividend(**data)
