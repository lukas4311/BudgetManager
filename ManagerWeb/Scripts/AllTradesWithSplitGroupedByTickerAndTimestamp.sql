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
        TradeSize * SplitAdjustment AS AdjustedTradeSize,
        TradeValue
    FROM
        TradesWithSplit
),
AggregatedTrades AS (
    SELECT
        StockTickerId,
        TradeTimeStamp,
        ISNULL(SUM(AdjustedTradeSize), 0) AS TotalTradeSize,
        ISNULL(SUM(TradeValue), 0) AS TotalTradeValue
    FROM
        AdjustedTrades
    GROUP BY
        StockTickerId,
        TradeTimeStamp
),
AccumulatedTrades AS (
    SELECT
        StockTickerId,
        TradeTimeStamp,
        TotalTradeSize,
        TotalTradeValue,
        SUM(TotalTradeSize) OVER (PARTITION BY StockTickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize
    FROM
        AggregatedTrades
)
SELECT
    StockTickerId,
    TradeTimeStamp,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AccumulatedTrades
ORDER BY
    StockTickerId, TradeTimeStamp;
