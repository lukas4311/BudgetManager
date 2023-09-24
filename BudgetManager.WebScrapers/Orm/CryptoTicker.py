from sqlalchemy import Column, Integer, String, Sequence
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class CryptoTicker(Base):
    __tablename__ = 'CryptoTicker'
    Id = Column(Integer, Sequence('id_seq'), primary_key=True)
    Ticker = Column(String(20), nullable=False)
    Name = Column(String(100), nullable=False)