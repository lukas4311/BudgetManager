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
		,'J�dlo'
		,'food'
	),
	(
		 'Shop'
		,'N�kupy'
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
		,'Bydlen�'
		,'realestate'
	),
	(
		 'Bills'
		,'Finan�n� ��ty'
		,'bill'
	),
	(
		 'Travel'
		,'Cestov�n�'
		,'travel'
	),
	(
		 'Fun'
		,'Z�bava'
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
		'n��ad�',
		'tools'
	),
	(
		'Medication',
		'l�ky',
		'medication'
	),
	(
		'Pets',
		'dom�c� mazl��ci',
		'pets'
	),
	(
		'Kids',
		'd�ti',
		'kids'
	),
	(
		'Education',
		'vzd�l�n�',
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
		'd�rky',
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
