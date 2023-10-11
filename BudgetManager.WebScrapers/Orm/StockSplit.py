from sqlalchemy import String, Numeric, Integer, DateTime
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class StockSplit(Base):
    __tablename__ = 'StockSplit'
    id: Mapped[int] = mapped_column(primary_key=True)
    stockTickerId: Mapped[int] = mapped_column(Integer, nullable=False)
    splitTimeStamp: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    splitTextInfo: Mapped[str] = mapped_column(String, nullable=False)
    splitCoefficient: Mapped[float] = mapped_column(Numeric, nullable=False)
