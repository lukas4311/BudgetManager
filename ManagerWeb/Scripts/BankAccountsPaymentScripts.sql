--*** GET ALL BANK ACCOUNT CURRENT BALANCE (SEPARATELY)

SELECT 
    ba.Id AS BankAccountId,
    ba.Code AS BankAccountCode,
    ba.OpeningBalance,
    ISNULL(SUM(p.Amount), 0) AS TotalPayments,
    ba.OpeningBalance + ISNULL(SUM(p.Amount), 0) AS CurrentBalance
FROM 
    BankAccount ba
LEFT JOIN 
    Payment p ON ba.Id = p.BankAccountId
GROUP BY 
    ba.Id, ba.Code, ba.OpeningBalance
ORDER BY 
    ba.Id;


--*** GET ALL PAYMENTS GROUPED BY MONTH AND YEAR (TOGETHER)
SELECT 
    YEAR(p.Date) AS Year,
    MONTH(p.Date) AS Month,
    SUM(p.Amount) AS TotalPayments
FROM 
    Payment p
GROUP BY 
    YEAR(p.Date), MONTH(p.Date)
ORDER BY 
    Year, Month;

--*** GET ALL PAYMENTS GROUPED BY MONTH AND YEAR (SEPARATELY)
SELECT 
    p.BankAccountId,
    YEAR(p.Date) AS Year,
    MONTH(p.Date) AS Month,
    SUM(p.Amount) AS TotalPayments
FROM 
    Payment p
GROUP BY 
    p.BankAccountId, YEAR(p.Date), MONTH(p.Date)
ORDER BY 
    p.BankAccountId, Year, Month;

--*** GET ALL PAYMENETS ACCUMULATED IN TIME

DECLARE @FromDate DATE = '2023-01-01'; -- Specify your from date
DECLARE @ToDate DATE = '2024-12-31'; -- Specify your to date

;WITH MonthSeries AS (
    SELECT 
        DATEFROMPARTS(YEAR(@FromDate), MONTH(@FromDate), 1) AS MonthStart
    UNION ALL
    SELECT 
        DATEADD(MONTH, 1, MonthStart)
    FROM 
        MonthSeries
    WHERE 
        MonthStart < DATEFROMPARTS(YEAR(@ToDate), MONTH(@ToDate), 1)
),
AllMonths AS (
    SELECT 
        ba.Id AS BankAccountId,
        ms.MonthStart,
        YEAR(ms.MonthStart) AS Year,
        MONTH(ms.MonthStart) AS Month
    FROM 
        BankAccount ba
    CROSS JOIN 
        MonthSeries ms
),
MonthlyPayments AS (
    SELECT 
        am.BankAccountId,
        am.Year,
        am.Month,
        ISNULL(SUM(p.Amount), 0) AS TotalPayments
    FROM 
        AllMonths am
    LEFT JOIN 
        Payment p ON am.BankAccountId = p.BankAccountId 
                   AND YEAR(p.Date) = am.Year 
                   AND MONTH(p.Date) = am.Month
    GROUP BY 
        am.BankAccountId, am.Year, am.Month
),
AccumulatedBalances AS (
    SELECT 
        mp.BankAccountId,
        mp.Year,
        mp.Month,
        mp.TotalPayments,
        SUM(mp.TotalPayments) OVER (PARTITION BY mp.BankAccountId ORDER BY mp.Year, mp.Month) AS AccumulatedBalance
    FROM 
        MonthlyPayments mp
)
SELECT 
    ab.BankAccountId,
    ab.Year,
    ab.Month,
    ab.TotalPayments,
    ab.AccumulatedBalance
FROM 
    AccumulatedBalances ab
ORDER BY 
    ab.BankAccountId, ab.Year, ab.Month;
	
--*** GET ALL PAYMENETS ACCUMULATED IN TIME (SEPARATELY)

DECLARE @FromDate DATE = '2023-01-01'; -- Specify your from date
DECLARE @ToDate DATE = '2024-12-31'; -- Specify your to date

;WITH MonthSeries AS (
    SELECT 
        DATEFROMPARTS(YEAR(@FromDate), MONTH(@FromDate), 1) AS MonthStart
    UNION ALL
    SELECT 
        DATEADD(MONTH, 1, MonthStart)
    FROM 
        MonthSeries
    WHERE 
        MonthStart < DATEFROMPARTS(YEAR(@ToDate), MONTH(@ToDate), 1)
),
AllMonths AS (
    SELECT 
        ba.Id AS BankAccountId,
        ms.MonthStart,
        YEAR(ms.MonthStart) AS Year,
        MONTH(ms.MonthStart) AS Month
    FROM 
        BankAccount ba
    CROSS JOIN 
        MonthSeries ms
),
MonthlyPayments AS (
    SELECT 
        am.BankAccountId,
        am.Year,
        am.Month,
        ISNULL(SUM(p.Amount), 0) AS TotalPayments
    FROM 
        AllMonths am
    LEFT JOIN 
        Payment p ON am.BankAccountId = p.BankAccountId 
                   AND YEAR(p.Date) = am.Year 
                   AND MONTH(p.Date) = am.Month
    GROUP BY 
        am.BankAccountId, am.Year, am.Month
),
AccumulatedBalances AS (
    SELECT 
        mp.BankAccountId,
        mp.Year,
        mp.Month,
        mp.TotalPayments,
        SUM(mp.TotalPayments) OVER (PARTITION BY mp.BankAccountId ORDER BY mp.Year, mp.Month) AS AccumulatedBalance
    FROM 
        MonthlyPayments mp
)
SELECT 
    ab.BankAccountId,
    ab.Year,
    ab.Month,
    ab.TotalPayments,
    ab.AccumulatedBalance
FROM 
    AccumulatedBalances ab
ORDER BY 
    ab.BankAccountId, ab.Year, ab.Month;
