/************************* seed comodity units **************************/
USE BudgetManager

DROP TABLE IF EXISTS #Units

CREATE TABLE #Units(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
)

INSERT INTO #Units
VALUES
	(
		'g'
		,'gramme'
	),
	(
		'l',
		'litre'
	)

INSERT INTO ComodityUnit(Code, Name)
SELECT
	NC.Code,
	NC.Name
FROM
	dbo.ComodityUnit AS PC
RIGHT JOIN #Units AS NC ON
	NC.Code = PC.Code
WHERE 
	PC.Id IS NULL


/************************* seed comodity types **************************/

DROP TABLE IF EXISTS #Types

DECLARE @comodityUnit INT
SET @comodityUnit = (SELECT Id FROM ComodityUnit WHERE Code = 'g')

CREATE TABLE #Types(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ComodityUnitId] INT NOT NULL
)

INSERT INTO #Types
VALUES
	(
		'AU'
		,'Gold'
		, @comodityUnit
	),
	(
		'AG',
		'Silver',
		@comodityUnit
	)

INSERT INTO ComodityType(Code, Name, ComodityUnitId)
SELECT
	NC.Code,
	NC.Name,
	NC.ComodityUnitId
FROM
	dbo.ComodityType AS PC
RIGHT JOIN #Types AS NC ON
	NC.Code = PC.Code
WHERE 
	PC.Id IS NULL

-- Insert into EnumItemType for TradeTicker
IF NOT EXISTS (SELECT 1 FROM [dbo].[EnumItemType] WHERE [Code] = 'TradeTicker')
BEGIN
    INSERT INTO [dbo].[EnumItemType] ([Code], [Name])
    VALUES ('TradeTicker', 'Trade Ticker');
    SELECT SCOPE_IDENTITY() AS NewEnumItemTypeId; -- Optionally retrieve the newly inserted Id
END
ELSE
BEGIN
    SELECT Id AS ExistingEnumItemTypeId FROM [dbo].[EnumItemType] WHERE [Code] = 'TradeTicker';
END

GO;

-- Ensure EnumItemType 'CommodityUnit' exists or insert it if not
IF NOT EXISTS (SELECT 1 FROM [dbo].[EnumItemType] WHERE [Code] = 'CommodityUnit')
BEGIN
    INSERT INTO [dbo].[EnumItemType] ([Code], [Name])
    VALUES ('CommodityUnit', 'Commodity Unit');
    SELECT SCOPE_IDENTITY() AS NewEnumItemTypeId; -- Optionally retrieve the newly inserted Id
END
ELSE
BEGIN
    SELECT Id AS ExistingEnumItemTypeId FROM [dbo].[EnumItemType] WHERE [Code] = 'CommodityUnit';
END

-- Temporary table to hold commodity unit data
DROP TABLE IF EXISTS #Units;
CREATE TABLE #Units(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
)

-- Insert data into #Units
INSERT INTO #Units
VALUES
    ('g', 'gram'),
    ('l', 'litre');

-- Insert data from #Units into EnumItem if not already inserted
INSERT INTO [dbo].[EnumItem] ([Code], [Name], [EnumItemTypeId], [Metadata])
SELECT u.[Code], u.[Name], et.[Id], NULL
FROM #Units u
JOIN [dbo].[EnumItemType] et ON et.[Code] = 'CommodityUnit'
LEFT JOIN [dbo].[EnumItem] ei ON ei.[Code] = u.[Code] AND ei.[EnumItemTypeId] = et.[Id]
WHERE ei.[Id] IS NULL;

-- Drop temporary table
DROP TABLE IF EXISTS #Units;