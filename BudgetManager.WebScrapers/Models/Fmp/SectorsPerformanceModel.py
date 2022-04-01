from dataclasses import dataclass


@dataclass
class SectorsPerformanceModel:
    sector: str
    changesPercentage: str

    @staticmethod
    def create_from_json(data):
        return SectorsPerformanceModel(**data)
