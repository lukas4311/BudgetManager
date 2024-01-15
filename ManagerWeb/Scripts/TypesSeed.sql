INSERT INTO dbo.PaymentType
(
	 Code
	,Name
)
VALUES
(
	 'Revenue'
	,'Revenue'
),
(
	 'Expense'
	,'Expense'
),
(
	 'Transfer'
	,'Transfer'
)

insert into [BudgetManager].[dbo].[BrokerReportType] (Code, Name)
values (N'Stock', N'Stock trades'), (N'Crypto', N'Crypto trades')

insert into [BudgetManager].[dbo].[BrokerReportToProcessState] (Code, Name)
values (N'InProcess', N'Report is in process'), (N'Finished', N'Report import process is finished'), (N'ParsingError', N'Error while parsing of report')