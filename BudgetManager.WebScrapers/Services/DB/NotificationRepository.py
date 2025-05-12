from sqlalchemy import create_engine, insert
import secret
from Services.DB.Orm.Notification import Notification

connectionString = f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes'


class NotificationRepository:
    def insert_stock_trade(self, notification: Notification):
        engine = create_engine(connectionString)
        insert_command = insert(Notification).values(heading=notification.heading, userIdentityId=1,
                                                     content=notification.content, isDisplayed=notification.isDisplayed,
                                                     timestamp=notification.timestamp,
                                                     attachmentUrl=notification.attachmentUrl)

        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()
