from sqlalchemy import create_engine, select
from sqlalchemy.orm import sessionmaker, Session
import pyodbc

import secret
from Orm.CurrencySymbol import Base, CurrencySymbol

print(pyodbc.drivers())

# Create an engine that connects to a local MSSQL database using a trusted connection.
# Replace 'localhost', 'port', and 'database_name' with your actual server details.
engine = create_engine(f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes')

# Create all tables in the engine (if they don't exist yet).
Base.metadata.create_all(engine)

# Create a configured "Session" class.
session = Session(engine)
stmt = select(CurrencySymbol).where(CurrencySymbol.symbol.in_(["USD"]))

for user in session.scalars(stmt):
    print(user.id)

# Print the Id and Symbol of each USD symbol.
# with engine.connect() as conn:
#    stmt = select(currencySymbol.symbol)
#    for row in conn.execute(stmt):
#         print(row)

# for row  in session.execute(select(currencySymbol.c.id, currencySymbol.c.symbol)).scalars().all():
#    print(row)

# Close the session.
session.close()