/************************* seed categories **************************/

DROP TABLE IF EXISTS #Categories

CREATE TABLE #Categories(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Icon] [nvarchar](20) NOT NULL,
)

INSERT INTO #Categories
VALUES
	(
		'Food'
		,'Food'
		,'food'
	),
	(
		 'Shop'
		,'Shop'
		,'shop'
	),
	(
		 'Transport'
		,'Transport'
		,'bus'
	),
	(
		 'Automoto'
		,'Automoto'
		,'car'
	),
	(
		 'Housing'
		,'Housing'
		,'realestate'
	),
	(
		 'Bills'
		,'Bills'
		,'bill'
	),
	(
		 'Travel'
		,'Travel'
		,'travel'
	),
	(
		 'Fun'
		,'Fun'
		,'fun'
	),
	(
		 'Culture'
		,'Culture'
		,'theatre'
	),
	(
		 'Invetsment'
		,'Invetsment'
		,'cash'
	),
	(
		 'Phone'
		,'Phone'
		,'mobile'
	),
	(
		 'Network'
		,'Network'
		,'net'
	),
	(
		'Tools',
		'Tools',
		'tools'
	),
	(
		'Medication',
		'Medication',
		'medication'
	),
	(
		'Pets',
		'Pets',
		'pets'
	),
	(
		'Kids',
		'Kids',
		'kids'
	),
	(
		'Education',
		'Education',
		'education'
	),
	(
		'Insurance',
		'Insurance',
		'insurance'
	),
	(
		 'Salary'
		,'Salary'
		,'salary'
	),
	(
		 'Sale'
		,'Sale'
		,'sale'
	),
	(
		'Alimony'
		,'Alimony'
		,'alimony'
	),
	(
		'Dividents',
		'Dividents',
		'dividents'
	),
	(
		'Games',
		'Games',
		'games'
	),
	(
		'Presents',
		'Presents',
		'presents'
	)

INSERT INTO PaymentCategory([Code], [Name], [Icon])
SELECT
	NC.Code,
	NC.Name,
	NC.Icon
FROM
	dbo.PaymentCategory AS PC
RIGHT JOIN #Categories AS NC ON
	NC.Code = PC.Code
WHERE 
	PC.Id IS NULL
