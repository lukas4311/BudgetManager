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
		,'Jídlo'
		,'food'
	),
	(
		 'Shop'
		,'Nákupy'
		,'shop'
	),
	(
		 'Transport'
		,'Doprava'
		,'bus'
	),
	(
		 'Automoto'
		,'Automoto'
		,'car'
	),
	(
		 'Housing'
		,'Bydlení'
		,'realestate'
	),
	(
		 'Bills'
		,'Finanèní úèty'
		,'bill'
	),
	(
		 'Travel'
		,'Cestování'
		,'travel'
	),
	(
		 'Fun'
		,'Zábava'
		,'fun'
	),
	(
		 'Culture'
		,'Kultura'
		,'theatre'
	),
	(
		 'Invetsment'
		,'Investice'
		,'cash'
	),
	(
		 'Phone'
		,'Mobil'
		,'mobile'
	),
	(
		 'Network'
		,'Internet'
		,'net'
	),
	(
		'Tools',
		'náøadí',
		'tools'
	),
	(
		'Medication',
		'léky',
		'medication'
	),
	(
		'Pets',
		'domácí mazlíèci',
		'pets'
	),
	(
		'Kids',
		'dìti',
		'kids'
	),
	(
		'Education',
		'vzdìlání',
		'education'
	),
	(
		'Insurance',
		'pojistka',
		'insurance'
	),
	(
		 'Salary'
		,'Mzda'
		,'salary'
	),
	(
		 'Sale'
		,'Prodej'
		,'sale'
	),
	(
		'Alimony'
		,'alimenty'
		,'alimony'
	),
	(
		'Dividents',
		'dividendy',
		'dividents'
	),
	(
		'Games',
		'hry',
		'games'
	),
	(
		'Presents',
		'dárky',
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
