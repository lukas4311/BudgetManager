--**** grouped by ticker
;WITH AdjustedTrades AS (
    SELECT
        cth.CryptoTickerId,
        cth.TradeTimeStamp,
        CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
        cth.TradeValue
    FROM
        [dbo].[CryptoTradeHistory] cth
),
AggregatedTrades AS (
    SELECT
        cth.CryptoTickerId,
        SUM(CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END) AS TotalTradeSize,
        SUM(cth.TradeValue) AS TotalTradeValue
    FROM
        [dbo].[CryptoTradeHistory] cth
    GROUP BY
        cth.CryptoTickerId
),
AccumulatedTrades AS (
    SELECT
        CryptoTickerId,
        TotalTradeSize,
        TotalTradeValue,
        SUM(TotalTradeSize) OVER (PARTITION BY CryptoTickerId) AS AccumulatedTradeSize
    FROM
        AggregatedTrades
)
SELECT
    CryptoTickerId,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AccumulatedTrades
ORDER BY
    CryptoTickerId


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
DECLARE @FromDate DATE = '2022-01-01';  -- Replace with your actual from date
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
Trades AS (
    SELECT
        cth.CryptoTickerId,
        cth.TradeTimeStamp,
        CASE WHEN cth.TradeValue >= 0 THEN -cth.TradeSize ELSE cth.TradeSize END AS AdjustedTradeSize,
        cth.TradeValue,
        YEAR(cth.TradeTimeStamp) AS TradeYear,
        MONTH(cth.TradeTimeStamp) AS TradeMonth
    FROM
        [dbo].[CryptoTradeHistory] cth
),
AggregatedTrades AS (
    SELECT
        ct.CryptoTickerId,
        YEAR(ms.MonthStart) AS TradeYear,
        MONTH(ms.MonthStart) AS TradeMonth,
        ISNULL(SUM(t.AdjustedTradeSize), 0) AS TradeSize,
        ISNULL(SUM(t.TradeValue), 0) AS TradeValue
    FROM
        MonthSeries ms
    CROSS JOIN
        CryptoTickers ct
    LEFT JOIN
        Trades t ON 
            ct.CryptoTickerId = t.CryptoTickerId AND
            t.TradeYear = YEAR(ms.MonthStart) AND 
            t.TradeMonth = MONTH(ms.MonthStart)
    GROUP BY
        ct.CryptoTickerId,
        YEAR(ms.MonthStart),
        MONTH(ms.MonthStart)
),
AccumulatedTrades AS (
    SELECT
        CryptoTickerId,
        TradeYear,
        TradeMonth,
        TradeSize,
        TradeValue,
        SUM(TradeSize) OVER (PARTITION BY CryptoTickerId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY CryptoTickerId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    CryptoTickerId,
    TradeYear,
    TradeMonth,
    AccumulatedTradeSize,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    CryptoTickerId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0);
