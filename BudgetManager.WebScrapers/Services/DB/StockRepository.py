import json

from sqlalchemy import create_engine, select, insert, and_, update, Null, text
from sqlalchemy.orm import Session
from typing import List, Optional
import secret
from Models.TradingReportData import TradingReportData
from Services.DB.Orm.BrokerReportToProcess import BrokerReportToProcess
from Services.DB.Orm.BrokerReportToProcessState import BrokerReportToProcessState
from Services.DB.Orm.EnumItem import EnumItem
from Services.DB.Orm.EnumItemType import EnumItemType
from Services.DB.Orm.StockTicker import Base
from Services.DB.Orm.Trade import Trade

# Global connection string using Windows authentication
connectionString = f'mssql+pyodbc://@{secret.serverName}/{secret.datebaseName}?driver=ODBC+Driver+17+for+SQL+Server&Trusted_Connection=yes'


class StockRepository:
    """
    Repository class for managing stock market data in the database.

    This class provides methods for:
    - Managing ticker symbols and their metadata
    - Storing and retrieving trading data
    - Handling broker reports and processing states
    - Working with enumeration types and items

    All database operations use SQLAlchemy ORM with automatic session management.
    """

    def get_enum_type(self, code: str) -> Optional[int]:
        """
        Retrieve the ID of an enumeration type by its code.

        Args:
            code (str): The code identifier for the enum type (e.g., 'TICKER_TYPE')

        Returns:
            Optional[int]: The ID of the enum type, or None if not found
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItemType).where(EnumItemType.code == code)
        trade_ticker_type = session.scalars(stmt).first()
        return trade_ticker_type.id if trade_ticker_type is not None else None

    def get_enums_by_type_id(self, enum_item_type_id: int) -> Optional[List[EnumItem]]:
        """
        Retrieve all enumeration items for a specific type ID.

        Args:
            enum_item_type_id (int): The ID of the enumeration type

        Returns:
            Optional[List[EnumItem]]: List of enum items, or None if none found
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItem).where(EnumItem.enumItemTypeId == enum_item_type_id)
        enums = session.scalars(stmt).all()
        return enums if enums is not None else None

    def get_ticker_id(self, ticker: str, ticker_type: str) -> Optional[int]:
        """
        Retrieve the ID of a ticker symbol by its code and type.

        Args:
            ticker (str): The ticker symbol (e.g., 'AAPL', 'GOOGL')
            ticker_type (str): The type code for the ticker category

        Returns:
            Optional[int]: The ID of the ticker, or None if not found
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        trade_ticker_type = self.get_enum_type(ticker_type)

        stmt = select(EnumItem).where(and_(EnumItem.code == ticker, EnumItem.enumItemTypeId == trade_ticker_type))
        ticker_model = session.scalars(stmt).first()

        return ticker_model.id if ticker_model is not None else None

    def get_all_tickers(self, ticker_type: str) -> List[EnumItem]:
        """
        Retrieve all ticker symbols of a specific type.

        Args:
            ticker_type (str): The type code for the ticker category

        Returns:
            List[EnumItem]: List of all tickers matching the specified type
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        trade_ticker_type = self.get_enum_type(ticker_type)

        stmt = select(EnumItem).where(EnumItem.enumItemTypeId == trade_ticker_type)
        return session.scalars(stmt).all()

    def _create_new_ticker(self, ticker: str, name: str, ticker_type: str, isin: Optional[str]) -> None:
        """
        Create a new ticker symbol in the database.

        Private method that inserts a new ticker with optional ISIN metadata.
        The ISIN (if provided) is stored as JSON metadata.

        Args:
            ticker (str): The ticker symbol code
            name (str): Full name of the security
            ticker_type (str): Type category for the ticker
            isin (Optional[str]): International Securities Identification Number
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        enum_item_type_ticker = self.get_enum_type(ticker_type)

        metadata = Null

        if isin is not None:
            data = {"isin": isin}
            metadata = json.dumps(data)

        insert_command = insert(EnumItem).values(
            code=ticker,
            name=name,
            enumItemTypeId=enum_item_type_ticker,
            _metadata=metadata
        )

        with engine.connect() as conn:
            conn.execute(insert_command)
            conn.commit()

    def update_ticker_metadata(self, ticker: str, ticker_type: str, metadata: dict) -> None:
        """
        Update the metadata for an existing ticker symbol.

        Updates the JSON metadata field for a ticker, allowing storage
        of additional information like ISIN, exchange details, etc.

        Args:
            ticker (str): The ticker symbol to update
            ticker_type (str): Type category for the ticker
            metadata (dict): Dictionary containing metadata to store as JSON
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        enum_item_type_ticker = self.get_enum_type(ticker_type)

        if metadata is not None:
            metadata_string = json.dumps(metadata)
            update_command = (update(EnumItem)
                              .where(and_(EnumItem.code == ticker,
                                          EnumItem.enumItemTypeId == enum_item_type_ticker))
                              .values(_metadata=metadata_string))

            with engine.connect() as conn:
                conn.execute(update_command)
                conn.commit()

    def get_currency_id(self, currency_code: str) -> Optional[int]:
        """
        Retrieve the ID of a currency by its code.

        Looks up currency symbols in the enumeration system to get
        the internal ID for database references.

        Args:
            currency_code (str): Currency code (e.g., 'USD', 'EUR', 'GBP')
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        stmt = select(EnumItemType).where(EnumItemType.code == 'CurrencySymbols')
        currency_symbols_type = session.scalars(stmt).first()

        stmt = select(EnumItem).where(
            EnumItem.code == currency_code and EnumItem.enumItemTypeId == currency_symbols_type)
        currency_model = session.scalars(stmt).first()
        return currency_model.id if currency_model is not None else None

    def _get_all_stock_broker_reports_to_process(self) -> List[BrokerReportToProcess]:
        """
        Retrieve all broker reports that are currently in processing state.

        Private method that finds all broker reports marked as "InProcess"
        for batch processing operations.

        Returns:
            List[BrokerReportToProcess]: List of broker reports to process
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        # Get the "InProcess" state ID
        broker_state_command = select(BrokerReportToProcessState).where(BrokerReportToProcessState.code == "InProcess")
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        # Find all reports with that state
        broker_report_data_command = select(BrokerReportToProcess).where(
            BrokerReportToProcess.brokerReportToProcessStateId == broker_state_id)

        broker_report_data = session.scalars(broker_report_data_command).all()
        session.close()

        return broker_report_data

    def _insert_stock_trade(self, trading_data: TradingReportData, user_id: int) -> None:
        """
        Insert a single stock trade into the database.

        Private method that creates a new trade record, checking for duplicates
        first to prevent inserting the same trade multiple times.

        Args:
            trading_data (TradingReportData): Trade information to insert
            user_id (int): ID of the user making the trade

        Raises:
            ValueError: If ticker doesn't exist or currency_id is None
        """
        ticker_id = int(self.get_ticker_id(trading_data.ticker, trading_data.trade_ticker_type_code))

        if ticker_id is None:
            print(f'Ticker does not exists {trading_data.ticker}')

        if trading_data.currency_id is None:
            print('Throw exception')

        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        # Check for duplicate trades
        stock_trade = (select(Trade)
                       .where(and_(Trade.tradeValue == trading_data.total,
                                   Trade.tradeSize == trading_data.number_of_shares,
                                   Trade.tickerId == ticker_id,
                                   Trade.tradeTimeStamp == trading_data.time)
                              ))
        stock_data = session.scalars(stock_trade)
        stock_trade = stock_data.first()

        if stock_trade is None:
            # Insert new trade
            insert_command = insert(Trade).values(
                tradeTimeStamp=trading_data.time,
                tickerId=ticker_id,
                tradeSize=trading_data.number_of_shares,
                tradeValue=trading_data.total,
                tradeCurrencySymbolId=trading_data.currency_id,
                userIdentityId=user_id,
                transactionId=trading_data.transaction_id
            )
            with engine.connect() as conn:
                conn.execute(insert_command)
                conn.commit()
        else:
            print('Trade is already saved.')

        session.close()

    def store_trade_data(self, trade_data: List[TradingReportData], user_id: int) -> None:
        """
        Store multiple trading records in the database.

        Public method that processes a list of trades, creating new tickers
        if necessary and inserting all trade records.

        Args:
            trade_data (List[TradingReportData]): List of trades to store
            user_id (int): ID of the user associated with these trades
        """
        for trade in trade_data:
            ticker_id = self.get_ticker_id(trade.ticker, trade.trade_ticker_type_code)

            # Create ticker if it doesn't exist
            if not ticker_id:
                self._create_new_ticker(trade.ticker, trade.name, trade.trade_ticker_type_code, trade.isin)

            # Insert the trade
            self._insert_stock_trade(trade, user_id)

    def changeProcessState(self, broker_report_id: int, state_code: str) -> None:
        """
        Change the processing state of a broker report.

        Updates the state of a broker report in the processing workflow,
        allowing tracking of report processing status.

        Args:
            broker_report_id (int): ID of the broker report to update
            state_code (str): New state code (e.g., 'Processed', 'Failed', 'InProcess')
        """
        engine = create_engine(connectionString)

        Base.metadata.create_all(engine)
        session = Session(engine)

        # Get the state ID from the code
        broker_state_command = select(BrokerReportToProcessState).where(
            BrokerReportToProcessState.code == state_code)
        broker_state = session.scalars(broker_state_command).first()
        broker_state_id = broker_state.id

        # Update the report state
        update_command = update(BrokerReportToProcess).where(
            BrokerReportToProcess.id == broker_report_id).values(
            brokerReportToProcessStateId=broker_state_id)

        with engine.connect() as conn:
            conn.execute(update_command)
            conn.commit()

        session.close()

    def get_ticker_id_by_isin(self, isin: str) -> Optional[int]:
        """
        Retrieve a ticker ID using its ISIN from JSON metadata.

        Uses SQL JSON functions to search for tickers by their ISIN
        stored in the metadata field as JSON.

        Args:
            isin (str): International Securities Identification Number

        Returns:
            Optional[int]: The ticker ID if found, None otherwise
        """
        engine = create_engine(connectionString)

        with Session(engine) as session:
            query = text("""
                SELECT Id
                FROM EnumItem
                WHERE JSON_VALUE(Metadata, '$.ISIN') = :isin
            """)

            result = session.execute(query, {"isin": isin})
            return result.first()