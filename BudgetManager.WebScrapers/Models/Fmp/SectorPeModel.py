from dataclasses import dataclass


@dataclass
class SectorPeModel:
    date: str
    sector: str
    exchange: str
    pe: str

    @staticmethod
    def create_from_json(data):
        return SectorPeModel(**data)
