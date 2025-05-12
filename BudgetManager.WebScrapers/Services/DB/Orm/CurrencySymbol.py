from sqlalchemy import String
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped


class Base(DeclarativeBase):
    pass


class CurrencySymbol(Base):
    __tablename__ = 'CurrencySymbol'

    id: Mapped[int] = mapped_column(primary_key=True)
    symbol: Mapped[str] = mapped_column(String(20), nullable=False)
