from sqlalchemy import Column, Integer, String, Sequence, MetaData, Table
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import mapped_column, DeclarativeBase


# class Base(DeclarativeBase):
#     pass
#
# class CurrencySymbol(Base):
#     __tablename__ = 'CurrencySymbol'
#
#     id = mapped_column(Integer, primary_key=True)
#     symbol = mapped_column(String(20), nullable=False)

metadata_obj = MetaData()


currencySymbol = Table(
    "CurrencySymbol",
    metadata_obj,
    Column("id", Integer, primary_key=True),
    Column("symbol", String(20)),
)