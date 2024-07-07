--**** grouped by ticker
;WITH AdjustedTrades AS (
    SELECT
        cth.CryptoTickerId,
        cth.CurrencySymbolId,
        cths.Symbol,
        cth.TradeTimeStamp,
        CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
        cth.TradeValue
    FROM
        [dbo].[CryptoTradeHistory] cth
    JOIN
        [dbo].[CurrencySymbol] cths ON cth.CurrencySymbolId = cths.Id
),
AggregatedTrades AS (
    SELECT
        at.CryptoTickerId,
        at.CurrencySymbolId,
        at.Symbol,
        SUM(CASE WHEN at.TradeValue >= 0 THEN -at.AdjustedTradeSize ELSE at.AdjustedTradeSize END) AS TotalTradeSize,
        SUM(at.TradeValue) AS TotalTradeValue
    FROM
        AdjustedTrades at
    GROUP BY
        at.CryptoTickerId,
        at.CurrencySymbolId,
        at.Symbol
),
AccumulatedTrades AS (
    SELECT
        CryptoTickerId,
        CurrencySymbolId,
        Symbol,
        TotalTradeSize,
        TotalTradeValue,
        SUM(TotalTradeSize) OVER (PARTITION BY CryptoTickerId ORDER BY CryptoTickerId) AS AccumulatedTradeSize
    FROM
        AggregatedTrades
)
SELECT
    CryptoTickerId,
    CurrencySymbolId,
    Symbol,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AccumulatedTrades
ORDER BY
    CryptoTickerId, CurrencySymbolId;


--**** grouped by trade date
DECLARE @FromDate DATE = '2022-01-01';  -- Replace with your actual from date
DECLARE @ToDate DATE = '2023-12-31';    -- Replace with your actual to date

;WITH AggregatedTrades AS (
    SELECT
        cth.CryptoTickerId,
        cth.TradeTimeStamp,
        SUM(CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END) AS TotalTradeSize,
        SUM(cth.TradeValue) AS TotalTradeValue,
        SUM(SUM(CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END)) OVER (PARTITION BY cth.CryptoTickerId ORDER BY cth.TradeTimeStamp) AS AccumulatedTradeSize
    FROM
        [dbo].[CryptoTradeHistory] cth
    WHERE
        cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
    GROUP BY
        cth.CryptoTickerId,
        cth.TradeTimeStamp
)
SELECT
    CryptoTickerId,
    TradeTimeStamp,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AggregatedTrades
ORDER BY
    CryptoTickerId, TradeTimeStamp;

--**** gouped by ticker and month
DECLARE @FromDate DATE = '2021-01-01';  -- Replace with your actual from date
DECLARE @ToDate DATE = '2023-12-31';    -- Replace with your actual to date

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
CryptoTickers AS (
    SELECT DISTINCT CryptoTickerId
    FROM [dbo].[CryptoTradeHistory]
),
UsedCurrencySymbols AS (
    SELECT DISTINCT
        cth.CryptoTickerId,
        cths.Id AS CurrencySymbolId,
        cths.Symbol AS CurrencySymbol
    FROM
        [dbo].[CryptoTradeHistory] cth
    JOIN
        [dbo].[CurrencySymbol] cths ON cth.CurrencySymbolId = cths.Id
    WHERE
        cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
),
MonthYearCrossJoin AS (
    SELECT
        ct.CryptoTickerId,
        ucs.CurrencySymbolId,
        ucs.CurrencySymbol,
        YEAR(ms.MonthStart) AS TradeYear,
        MONTH(ms.MonthStart) AS TradeMonth
    FROM
        MonthSeries ms
    CROSS JOIN
        CryptoTickers ct
    CROSS JOIN
        UsedCurrencySymbols ucs
    WHERE
        EXISTS (
            SELECT 1
            FROM [dbo].[CryptoTradeHistory] cth
            WHERE cth.CryptoTickerId = ct.CryptoTickerId
            AND cth.CurrencySymbolId = ucs.CurrencySymbolId
            AND cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
        )
),
AggregatedTrades AS (
    SELECT
        my.CryptoTickerId,
        my.CurrencySymbolId,
        my.CurrencySymbol,
        my.TradeYear,
        my.TradeMonth,
        ISNULL(SUM(t.AdjustedTradeSize), 0) AS TradeSize,
        ISNULL(SUM(t.TradeValue), 0) AS TradeValue
    FROM
        MonthYearCrossJoin my
    LEFT JOIN
        (
            SELECT
                cth.CryptoTickerId,
                cth.CurrencySymbolId,
                cths.Symbol AS CurrencySymbol,
                cth.TradeTimeStamp,
                CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
                cth.TradeValue,
                YEAR(cth.TradeTimeStamp) AS TradeYear,
                MONTH(cth.TradeTimeStamp) AS TradeMonth
            FROM
                [dbo].[CryptoTradeHistory] cth
            JOIN
                [dbo].[CurrencySymbol] cths ON cth.CurrencySymbolId = cths.Id
            WHERE
                cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
        ) t ON 
            my.CryptoTickerId = t.CryptoTickerId AND
            my.CurrencySymbolId = t.CurrencySymbolId AND
            my.TradeYear = t.TradeYear AND 
            my.TradeMonth = t.TradeMonth
    GROUP BY
        my.CryptoTickerId,
        my.CurrencySymbolId,
        my.CurrencySymbol,
        my.TradeYear,
        my.TradeMonth
),
AccumulatedTrades AS (
    SELECT
        CryptoTickerId,
        CurrencySymbolId,
        CurrencySymbol,
        TradeYear,
        TradeMonth,
        TradeSize,
        TradeValue,
        SUM(TradeSize) OVER (PARTITION BY CryptoTickerId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY CryptoTickerId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    CryptoTickerId,
    CurrencySymbolId,
    CurrencySymbol,
    TradeYear,
    TradeMonth,
    AccumulatedTradeSize,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    CryptoTickerId, CurrencySymbolId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0)