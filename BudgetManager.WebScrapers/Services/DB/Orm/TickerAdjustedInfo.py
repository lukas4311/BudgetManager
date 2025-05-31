from sqlalchemy import String, Text
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column


class Base(DeclarativeBase):
    pass


class TickerAdjustedInfo(Base):
    __tablename__ = 'TickerAdjustedInfo'
    __table_args__ = {'schema': 'dbo'}

    id: Mapped[int] = mapped_column(primary_key=True)
    priceTicker: Mapped[str] = mapped_column(String, nullable=True)
    companyInfoTicker: Mapped[str] = mapped_column(String, nullable=True)
    _metadata: Mapped[str] = mapped_column(String, nullable=True, name="metadata")
