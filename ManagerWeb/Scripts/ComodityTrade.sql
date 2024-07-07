--**** group by comodity type
SELECT
    cth.ComodityTypeId,
    cth.CurrencySymbolId,
    cs.Symbol,
    SUM(CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END) AS TotalTradeSize,
    SUM(cth.TradeValue) AS TotalTradeValue
FROM
    [dbo].[ComodityTradeHistory] cth
JOIN
    [dbo].[CurrencySymbol] cs ON cth.CurrencySymbolId = cs.Id
GROUP BY
    cth.ComodityTypeId,
    cth.CurrencySymbolId,
    cs.Symbol
ORDER BY
    cth.ComodityTypeId,
    cth.CurrencySymbolId;


--**** monthly grouped and accumulated
DECLARE @FromDate DATE = '2020-01-01';  -- Replace with your actual from date
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
ComodityTypes AS (
    SELECT DISTINCT ComodityTypeId
    FROM [dbo].[ComodityTradeHistory]
),
UsedCurrencySymbols AS (
    SELECT DISTINCT
        cth.ComodityTypeId,
        cths.Id as CurrencySymbolId,
        cths.Symbol AS CurrencySymbol
    FROM
        [dbo].[ComodityTradeHistory] cth
    JOIN
        [dbo].[CurrencySymbol] cths ON cth.CurrencySymbolId = cths.Id
    WHERE
        cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
),
MonthYearCrossJoin AS (
    SELECT
        ct.ComodityTypeId,
        ucs.CurrencySymbolId,
        ucs.CurrencySymbol,
        YEAR(ms.MonthStart) AS TradeYear,
        MONTH(ms.MonthStart) AS TradeMonth
    FROM
        MonthSeries ms
    CROSS JOIN
        ComodityTypes ct
    CROSS JOIN
        UsedCurrencySymbols ucs
    WHERE
        EXISTS (
            SELECT 1
            FROM [dbo].[ComodityTradeHistory] cth
            WHERE cth.ComodityTypeId = ct.ComodityTypeId
            AND cth.CurrencySymbolId = ucs.CurrencySymbolId
            AND cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
        )
),
AggregatedTrades AS (
    SELECT
        my.ComodityTypeId,
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
                cth.ComodityTypeId,
                cth.CurrencySymbolId,
                cths.Symbol AS CurrencySymbol,
                cth.TradeTimeStamp,
                CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
                cth.TradeValue,
                YEAR(cth.TradeTimeStamp) AS TradeYear,
                MONTH(cth.TradeTimeStamp) AS TradeMonth
            FROM
                [dbo].[ComodityTradeHistory] cth
            JOIN
                [dbo].[CurrencySymbol] cths ON cth.CurrencySymbolId = cths.Id
            WHERE
                cth.TradeTimeStamp BETWEEN @FromDate AND @ToDate
        ) t ON 
            my.ComodityTypeId = t.ComodityTypeId AND
            my.CurrencySymbolId = t.CurrencySymbolId AND
            my.TradeYear = t.TradeYear AND 
            my.TradeMonth = t.TradeMonth
    GROUP BY
        my.ComodityTypeId,
        my.CurrencySymbolId,
        my.CurrencySymbol,
        my.TradeYear,
        my.TradeMonth
),
AccumulatedTrades AS (
    SELECT
        ComodityTypeId,
        CurrencySymbolId,
        CurrencySymbol,
        TradeYear,
        TradeMonth,
        SUM(TradeSize) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    at.ComodityTypeId,
    at.CurrencySymbolId,
    at.CurrencySymbol,
    at.TradeYear,
    at.TradeMonth,
    at.AccumulatedTradeSize,
    at.AccumulatedTradeValue
FROM
    AccumulatedTrades at
ORDER BY
    at.ComodityTypeId, at.CurrencySymbolId, at.TradeYear, at.TradeMonth
OPTION (MAXRECURSION 0);