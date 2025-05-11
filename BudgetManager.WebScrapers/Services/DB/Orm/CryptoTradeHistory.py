from sqlalchemy import String, Integer, DateTime, Numeric
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class CryptoTradeHistory(Base):
    __tablename__ = 'CryptoTradeHistory'
    id: Mapped[int] = mapped_column(primary_key=True)
    tradeTimeStamp: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    cryptoTickerId: Mapped[int] = mapped_column(Integer, nullable=False)
    tradeValue: Mapped[float] = mapped_column(Numeric, nullable=False)
    currencySymbolId = mapped_column(Integer, nullable=False)
    tradeSize: Mapped[float] = mapped_column(Numeric, nullable=False)
    userIdentityId: Mapped[int] = mapped_column(Integer, nullable=False)