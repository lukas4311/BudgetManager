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
StockTickers AS (
    SELECT DISTINCT StockTickerId
    FROM [dbo].[StockTradeHistory]
),
TradesWithSplit AS (
    SELECT
        sth.StockTickerId,
        sth.TradeTimeStamp,
        sth.TradeSize,
        sth.TradeValue,
        COALESCE((
            SELECT EXP(SUM(LOG(ss.SplitCoefficient)))
            FROM [dbo].[StockSplit] ss
            WHERE ss.StockTickerId = sth.StockTickerId
              AND ss.SplitTimeStamp > sth.TradeTimeStamp
        ), 1) AS SplitAdjustment,
        YEAR(sth.TradeTimeStamp) AS TradeYear,
        MONTH(sth.TradeTimeStamp) AS TradeMonth
    FROM
        [dbo].[StockTradeHistory] sth
),
AggregatedTrades AS (
    SELECT
        st.StockTickerId,
        YEAR(ms.MonthStart) AS TradeYear,
        MONTH(ms.MonthStart) AS TradeMonth,
        ISNULL(SUM(CASE WHEN tws.TradeValue >= 0 THEN -tws.TradeSize * tws.SplitAdjustment ELSE tws.TradeSize * tws.SplitAdjustment END), 0) AS TradeSize,
        ISNULL(SUM(tws.TradeValue), 0) AS TradeValue
    FROM
        MonthSeries ms
    CROSS JOIN
        StockTickers st
    LEFT JOIN
        TradesWithSplit tws ON 
            st.StockTickerId = tws.StockTickerId AND
            tws.TradeYear = YEAR(ms.MonthStart) AND 
            tws.TradeMonth = MONTH(ms.MonthStart)
    GROUP BY
        st.StockTickerId,
        YEAR(ms.MonthStart),
        MONTH(ms.MonthStart)
),
AccumulatedTrades AS (
    SELECT
        StockTickerId,
        TradeYear,
        TradeMonth,
        TradeSize,
        TradeValue,
        SUM(TradeSize) OVER (PARTITION BY StockTickerId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize
    FROM
        AggregatedTrades
)
SELECT
    StockTickerId,
    TradeYear,
    TradeMonth,
    TradeSize,
    TradeValue,
    AccumulatedTradeSize
FROM
    AccumulatedTrades
ORDER BY
    StockTickerId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0);