from sqlalchemy import String, Integer, DateTime, Numeric
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class StockTradeHistory(Base):
    __tablename__ = 'StockTradeHistory'
    id: Mapped[int] = mapped_column(primary_key=True)
    tradeTimeStamp: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    stockTickerId: Mapped[int] = mapped_column(Integer, nullable=False)
    tradeSize: Mapped[float] = mapped_column(Numeric, nullable=False)
    tradeValue: Mapped[float] = mapped_column(Numeric, nullable=False)
    currencySymbolId: Mapped[int] = mapped_column(Integer, nullable=False)
    userIdentityId: Mapped[int] = mapped_column(Integer, nullable=False)
