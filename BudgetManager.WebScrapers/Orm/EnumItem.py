from sqlalchemy import String, Integer
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped

class Base(DeclarativeBase):
    pass

class EnumItem(Base):
    __tablename__ = 'EnumItem'

    id: Mapped[int] = mapped_column(primary_key=True)
    code: Mapped[str] = mapped_column(String, nullable=False)
    name: Mapped[str] = mapped_column(String, nullable=False)
    enumItemTypeId: Mapped[int] = mapped_column(Integer, nullable=False)
    metadata: Mapped[str] = mapped_column(String, nullable=False)
