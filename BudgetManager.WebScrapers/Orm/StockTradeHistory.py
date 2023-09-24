from sqlalchemy import Column, Integer, Float, DateTime, Sequence
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class StockTradeHistory(Base):
    __tablename__ = 'StockTradeHistory'
    Id = Column(Integer, Sequence('id_seq'), primary_key=True)
    TradeTimeStamp = Column(DateTime, nullable=False)
    StockTickerId = Column(Integer, nullable=False)
    TradeSize = Column(Float, nullable=False)
    TradeValue = Column(Float, nullable=False)
    CurrencySymbolId = Column(Integer, nullable=False)
    UserIdentityId = Column(Integer, nullable=False)