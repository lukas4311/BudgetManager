from sqlalchemy import String
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped


class Base(DeclarativeBase):
    pass


class CryptoTicker(Base):
    __tablename__ = 'CryptoTicker'
    id: Mapped[int] = mapped_column(primary_key=True)
    ticker: Mapped[str] = mapped_column(String(20), nullable=False)
    name: Mapped[str] = mapped_column(String(100), nullable=False)
