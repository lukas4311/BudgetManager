from sqlalchemy import String, DateTime, Integer
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class BrokerReportToProcessState(Base):
    __tablename__ = 'BrokerReportToProcessState'

    id: Mapped[int] = mapped_column(primary_key=True)
    importedTime: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    fileContentBase64: Mapped[str] = mapped_column(String, nullable=False)
    brokerReportToProcessStateId: Mapped[int] = mapped_column(Integer, nullable=False)
    userIdentityId: Mapped[int] = mapped_column(Integer, nullable=False)
    brokerReportTypeId: Mapped[int] = mapped_column(Integer, nullable=False)
