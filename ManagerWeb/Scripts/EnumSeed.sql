-- Insert into EnumItemType if not exists
IF NOT EXISTS (SELECT 1 FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols')
BEGIN
    INSERT INTO [dbo].[EnumItemType] ([Code], [Name])
    VALUES ('CurrencySymbols', 'Currency Symbols');
END

-- Insert into EnumItem if not exists
INSERT INTO [dbo].[EnumItem] ([Code], [Name], [EnumItemTypeId], [Metadata])
SELECT *
FROM (
    VALUES
        ('BTC', 'Bitcoin', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL),
        ('CZK', 'Czech Koruna', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL),
        ('ETH', 'Ethereum', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL),
        ('EUR', 'Euro', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL),
        ('USD', 'US Dollar', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL),
        ('USDC', 'USD Coin', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'CurrencySymbols'), NULL)
) AS EnumItems (Code, Name, EnumItemTypeId, Metadata)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[EnumItem] WHERE [Code] = EnumItems.Code);

GO;

-- Ensure EnumItemType 'PaymentCategory' exists or insert it if not
IF NOT EXISTS (SELECT 1 FROM [dbo].[EnumItemType] WHERE [Code] = 'PaymentCategory')
BEGIN
    INSERT INTO [dbo].[EnumItemType] ([Code], [Name])
    VALUES ('PaymentCategory', 'Payment Category');
END

-- Temporary table to hold categories data
DROP TABLE IF EXISTS #Categories;
CREATE TABLE #Categories(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Icon] [nvarchar](20) NOT NULL,
)

-- Insert data into #Categories
INSERT INTO #Categories
VALUES
    ('Food', 'Food', 'food'),
    ('Shop', 'Shop', 'shop'),
    ('Transport', 'Transport', 'bus'),
    ('Automoto', 'Automoto', 'car'),
    ('Housing', 'Housing', 'realestate'),
    ('Bills', 'Bills', 'bill'),
    ('Travel', 'Travel', 'travel'),
    ('Fun', 'Fun', 'fun'),
    ('Culture', 'Culture', 'theatre'),
    ('Investment', 'Investment', 'cash'),
    ('Phone', 'Phone', 'mobile'),
    ('Network', 'Network', 'net'),
    ('Tools', 'Tools', 'tools'),
    ('Medication', 'Medication', 'medication'),
    ('Pets', 'Pets', 'pets'),
    ('Kids', 'Kids', 'kids'),
    ('Education', 'Education', 'education'),
    ('Insurance', 'Insurance', 'insurance'),
    ('Salary', 'Salary', 'salary'),
    ('Sale', 'Sale', 'sale'),
    ('Alimony', 'Alimony', 'alimony'),
    ('Dividends', 'Dividends', 'dividends'),
    ('Games', 'Games', 'games'),
    ('Presents', 'Presents', 'presents');

-- Insert data from #Categories into EnumItem if not already inserted
INSERT INTO [dbo].[EnumItem] ([Code], [Name], [EnumItemTypeId], [Metadata])
SELECT c.[Code], c.[Name], et.[Id], 
    '{"Icon": "' + c.[Icon] + '"}' AS Metadata
FROM #Categories c
JOIN [dbo].[EnumItemType] et ON et.[Code] = 'PaymentCategory'
LEFT JOIN [dbo].[EnumItem] ei ON ei.[Code] = c.[Code] AND ei.[EnumItemTypeId] = et.[Id]
WHERE ei.[Id] IS NULL;

-- Drop temporary table
DROP TABLE IF EXISTS #Categories;

GO;

-- Insert into EnumItemType if not exists
IF NOT EXISTS (SELECT 1 FROM [dbo].[EnumItemType] WHERE [Code] = 'AvailableBrokerParsers')
BEGIN
    INSERT INTO [dbo].[EnumItemType] ([Code], [Name])
    VALUES ('AvailableBrokerParsers', 'Parser available for processing broker reports');
END

-- Insert into EnumItem if not exists
INSERT INTO [dbo].[EnumItem] ([Code], [Name], [EnumItemTypeId], [Metadata])
SELECT *
FROM (
    VALUES
        ('Trading212', 'Trading212', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'AvailableBrokerParsers'), NULL),
        ('InteractiveBrokers', 'Interactive Brokers', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'AvailableBrokerParsers'), NULL),
        ('Degiro', 'Degiro', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'AvailableBrokerParsers'), NULL),
        ('XTB', 'XTB', (SELECT Id FROM [dbo].[EnumItemType] WHERE [Code] = 'AvailableBrokerParsers'), NULL)
) AS EnumItems (Code, Name, EnumItemTypeId, Metadata)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[EnumItem] WHERE [Code] = EnumItems.Code);