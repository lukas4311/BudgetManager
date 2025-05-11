from sqlalchemy import String, Integer, DateTime, Boolean
from sqlalchemy.orm import mapped_column, DeclarativeBase, Mapped
import datetime


class Base(DeclarativeBase):
    pass


class Notification(Base):
    __tablename__ = 'Notification'

    id: Mapped[int] = mapped_column(primary_key=True)
    userIdentityId: Mapped[int] = mapped_column(Integer, nullable=False)
    heading: Mapped[str] = mapped_column(String, nullable=False)
    content: Mapped[str] = mapped_column(String, nullable=False)
    isDisplayed: Mapped[bool] = mapped_column(Boolean, nullable=False)
    timestamp: Mapped[datetime.datetime] = mapped_column(DateTime(timezone=True), nullable=False)
    attachmentUrl: Mapped[str] = mapped_column(String, nullable=False)
