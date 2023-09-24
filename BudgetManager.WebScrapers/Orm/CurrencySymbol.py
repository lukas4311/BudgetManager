from sqlalchemy import Column, Integer, String, Sequence
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class CurrencySymbol(Base):
    __tablename__ = 'CurrencySymbol'
    Id = Column(Integer, Sequence('id_seq'), primary_key=True)
    Symbol = Column(String(20), nullable=False)