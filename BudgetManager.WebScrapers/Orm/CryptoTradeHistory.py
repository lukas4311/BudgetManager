from sqlalchemy import Column, Integer, Float, DateTime, Sequence
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class CryptoTradeHistory(Base):
    __tablename__ = 'CryptoTradeHistory'
    Id = Column(Integer, Sequence('id_seq'), primary_key=True)
    TradeTimeStamp = Column(DateTime, nullable=False)
    CryptoTickerId = Column(Integer, nullable=False)
    TradeValue = Column(Float, nullable=False)
    CurrencySymbolId = Column(Integer, nullable=False)
    TradeSize = Column(Float, nullable=False)
    UserIdentityId = Column(Integer, nullable=False)