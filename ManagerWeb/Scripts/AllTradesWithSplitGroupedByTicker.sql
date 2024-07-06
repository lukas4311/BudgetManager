DECLARE @FromDate DATE = '2022-01-01';  -- Replace with your actual from date
DECLARE @ToDate DATE = '2023-12-31';    -- Replace with your actual to date

;WITH TradesWithSplit AS (
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
        ), 1) AS SplitAdjustment
    FROM
        [dbo].[StockTradeHistory] sth
),
AdjustedTrades AS (
    SELECT
        StockTickerId,
        TradeTimeStamp,
        CASE WHEN TradeValue >= 0 THEN -TradeSize * SplitAdjustment ELSE TradeSize * SplitAdjustment END AS AdjustedTradeSize,
        TradeValue
    FROM
        TradesWithSplit
),
AggregatedTrades AS (
    SELECT
        StockTickerId,
        ISNULL(SUM(AdjustedTradeSize), 0) AS TotalTradeSize,
        ISNULL(SUM(TradeValue), 0) AS TotalTradeValue
    FROM
        AdjustedTrades
    GROUP BY
        StockTickerId
),
AccumulatedTrades AS (
    SELECT
        StockTickerId,
        TradeTimeStamp,
        AdjustedTradeSize,
        SUM(AdjustedTradeSize) OVER (PARTITION BY StockTickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize
    FROM
        AdjustedTrades
)
SELECT
    AT.StockTickerId,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AggregatedTrades AS AT
JOIN
    (SELECT StockTickerId, MAX(TradeTimeStamp) AS LastTradeTimeStamp
     FROM AdjustedTrades
     GROUP BY StockTickerId) AS lastTrade
ON
    AT.StockTickerId = lastTrade.StockTickerId
JOIN
    AccumulatedTrades acc
ON
    lastTrade.StockTickerId = acc.StockTickerId
    AND lastTrade.LastTradeTimeStamp = acc.TradeTimeStamp
ORDER BY
    StockTickerId;