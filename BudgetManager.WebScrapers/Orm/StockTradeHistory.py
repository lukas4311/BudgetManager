from sqlalchemy import String, Integer, DateTime, Numeric
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class StockTradeHistory(Base):
    __tablename__ = 'StockTradeHistory'
    id: Mapped[int] = mapped_column(primary_key=True)
    tradeTimeStamp: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    StockTickerId: Mapped[int] = mapped_column(Integer, nullable=False)
    TradeSize: Mapped[float] = mapped_column(Numeric, nullable=False)
    TradeValue: Mapped[float] = mapped_column(Numeric, nullable=False)
    CurrencySymbolId: Mapped[int] = mapped_column(Integer, nullable=False)
    UserIdentityId: Mapped[int] = mapped_column(Integer, nullable=False)
