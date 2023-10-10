from sqlalchemy import String, Numeric
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped


class Base(DeclarativeBase):
    pass


class CryptoTicker(Base):
    __tablename__ = 'StockSplit'
    id: Mapped[int] = mapped_column(primary_key=True)
    splitTextInfo: Mapped[str] = mapped_column(String, nullable=False)
    splitCoefficient: Mapped[float] = mapped_column(Numeric, nullable=False)
