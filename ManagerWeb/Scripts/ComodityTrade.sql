--**** group by comodity type
SELECT
    ComodityTypeId,
    CurrencySymbolId,
    SUM(CASE WHEN TradeValue >= 0 THEN -TradeSize ELSE TradeSize END) AS TotalTradeSize,
    SUM(TradeValue) AS TotalTradeValue
FROM
    [dbo].[ComodityTradeHistory]
GROUP BY
    ComodityTypeId,
    CurrencySymbolId
ORDER BY
    ComodityTypeId,
    CurrencySymbolId

--**** monthly grouped and accumulated
DECLARE @FromDate DATE = '2021-01-01';  -- Replace with your actual from date
DECLARE @ToDate DATE = '2024-12-31';    -- Replace with your actual to date

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
CurrencySymbols AS (
    SELECT DISTINCT CurrencySymbolId
    FROM [dbo].[ComodityTradeHistory]
),
Trades AS (
    SELECT
        cth.ComodityTypeId,
        cth.CurrencySymbolId,
        cth.TradeTimeStamp,
        CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
        cth.TradeValue,
        YEAR(cth.TradeTimeStamp) AS TradeYear,
        MONTH(cth.TradeTimeStamp) AS TradeMonth
    FROM
        [dbo].[ComodityTradeHistory] cth
),
AggregatedTrades AS (
    SELECT
        ct.ComodityTypeId,
        cs.CurrencySymbolId,
        YEAR(ms.MonthStart) AS TradeYear,
        MONTH(ms.MonthStart) AS TradeMonth,
        ISNULL(SUM(t.AdjustedTradeSize), 0) AS TradeSize,
        ISNULL(SUM(t.TradeValue), 0) AS TradeValue
    FROM
        MonthSeries ms
    CROSS JOIN
        ComodityTypes ct
    CROSS JOIN
        CurrencySymbols cs
    LEFT JOIN
        Trades t ON 
            ct.ComodityTypeId = t.ComodityTypeId AND
            cs.CurrencySymbolId = t.CurrencySymbolId AND
            t.TradeYear = YEAR(ms.MonthStart) AND 
            t.TradeMonth = MONTH(ms.MonthStart)
    GROUP BY
        ct.ComodityTypeId,
        cs.CurrencySymbolId,
        YEAR(ms.MonthStart),
        MONTH(ms.MonthStart)
),
AccumulatedTrades AS (
    SELECT
        ComodityTypeId,
        CurrencySymbolId,
        TradeYear,
        TradeMonth,
        SUM(TradeSize) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    ComodityTypeId,
    CurrencySymbolId,
    TradeYear,
    TradeMonth,
    AccumulatedTradeSize,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    ComodityTypeId, CurrencySymbolId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0)
