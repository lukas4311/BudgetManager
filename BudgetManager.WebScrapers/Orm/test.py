from sqlalchemy import create_engine, select
from sqlalchemy.orm import sessionmaker, Session
import pyodbc

import secret
from Orm.CryptoTradeHistory import CryptoTradeHistory
from Orm.CurrencySymbol import Base, CurrencySymbol

# Create an engine that connects to a local MSSQL database using a trusted connection.
engine = create_engine(f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

# Create all tables in the engine (if they don't exist yet).
Base.metadata.create_all(engine)

session = Session(engine)
stmt = select(CurrencySymbol).where(CurrencySymbol.symbol.in_(["USD"]))

for user in session.scalars(stmt):
    print(user.id)

stmt2 = select(CryptoTradeHistory)

for cryptoData in session.scalars(stmt2):
    print(cryptoData.id, cryptoData.tradeTimeStamp)

# Close the session.
session.close()