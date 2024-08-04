using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class StockTradeQueries
    {
        public static FormattableString GetAllTradesWithSplitGroupedByMonthAndTicker(DateTime from, DateTime to)
        {
            return $@"
;WITH MonthSeries AS (
    SELECT 
        DATEFROMPARTS(YEAR({from:yyyy-MM-dd}), MONTH({from:yyyy-MM-dd}), 1) AS MonthStart
    UNION ALL
    SELECT 
        DATEADD(MONTH, 1, MonthStart)
    FROM 
        MonthSeries
    WHERE 
        MonthStart < DATEFROMPARTS(YEAR({to:yyyy-MM-dd}), MONTH({to:yyyy-MM-dd}), 1)
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
        ISNULL(SUM(tws.TradeSize * tws.SplitAdjustment), 0) AS TradeSize,
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
OPTION (MAXRECURSION 0)";
        }

        public static FormattableString GetAllTradesGroupedByTicker()
        {
            return $@"
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
    StockTickerId";
        }

        public static FormattableString GetAllTradesGroupedByTickerAndTradeDate()
        {
            return $@"
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
    StockTickerId, TradeTimeStamp";
        }

        public static FormattableString GetAllTradesGroupedByTickerAndTradeDate__TradeTable(int userId, TickerTypes tickersType)
        {
            return $@"
;WITH TradesWithSplit AS (
    SELECT
        sth.TickerId,
        sth.TradeTimeStamp,
        sth.TradeSize,
        sth.TradeValue,
        COALESCE((
            SELECT EXP(SUM(LOG(ss.SplitCoefficient)))
            FROM [dbo].[StockSplit] ss
            WHERE ss.TickerId = sth.TickerId
              AND ss.SplitTimeStamp > sth.TradeTimeStamp
        ), 1) AS SplitAdjustment
    FROM
        [dbo].[Trade] sth
	JOIN [dbo].[EnumItem] ei ON
		ei.Id = sth.TickerId
	JOIN dbo.EnumItemType eit ON
		eit.Id = ei.EnumItemTypeId
		AND eit.Code = {tickersType.ToString()}
    WHERE
		sth.UserIdentityId = {userId}
),
AdjustedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TradeSize * SplitAdjustment AS AdjustedTradeSize,
        TradeValue
    FROM
        TradesWithSplit
),
AggregatedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        ISNULL(SUM(AdjustedTradeSize), 0) AS TotalTradeSize,
        ISNULL(SUM(TradeValue), 0) AS TotalTradeValue
    FROM
        AdjustedTrades
    GROUP BY
        TickerId,
        TradeTimeStamp
),
AccumulatedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TotalTradeSize,
        TotalTradeValue,
        SUM(TotalTradeSize) OVER (PARTITION BY TickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize
    FROM
        AggregatedTrades
)
SELECT
    TickerId,
    TradeTimeStamp,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AccumulatedTrades
ORDER BY
    TickerId, TradeTimeStamp";
        }

        public static FormattableString GetAllTradesGroupedByTicker__TradeTable(int userId, TickerTypes tickersType)
        {
            return $@"
;WITH TradesWithSplit AS (
    SELECT
        sth.TickerId,
        sth.TradeTimeStamp,
        sth.TradeSize,
        sth.TradeValue,
        COALESCE((
            SELECT EXP(SUM(LOG(ss.SplitCoefficient)))
            FROM [dbo].[StockSplit] ss
            WHERE ss.TickerId = sth.TickerId
              AND ss.SplitTimeStamp > sth.TradeTimeStamp
        ), 1) AS SplitAdjustment
    FROM
        [dbo].[Trade] sth
	JOIN [dbo].[EnumItem] ei ON
		ei.Id = sth.TickerId
	JOIN dbo.EnumItemType eit ON
		eit.Id = ei.EnumItemTypeId
		AND eit.Code = {tickersType.ToString()}
    WHERE
		sth.UserIdentityId = {userId}
),
AdjustedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TradeSize * SplitAdjustment AS AdjustedTradeSize,
        TradeValue
    FROM
        TradesWithSplit
),
AggregatedTrades AS (
    SELECT
        TickerId,
        ISNULL(SUM(AdjustedTradeSize), 0) AS TotalTradeSize,
        ISNULL(SUM(TradeValue), 0) AS TotalTradeValue
    FROM
        AdjustedTrades
    GROUP BY
        TickerId
),
AccumulatedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        AdjustedTradeSize,
        SUM(AdjustedTradeSize) OVER (PARTITION BY TickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize
    FROM
        AdjustedTrades
)
SELECT
    AT.TickerId,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize
FROM
    AggregatedTrades AS AT
JOIN
    (SELECT TickerId, MAX(TradeTimeStamp) AS LastTradeTimeStamp
     FROM AdjustedTrades
     GROUP BY TickerId) AS lastTrade
ON
    AT.TickerId = lastTrade.TickerId
JOIN
    AccumulatedTrades acc
ON
    lastTrade.TickerId = acc.TickerId
    AND lastTrade.LastTradeTimeStamp = acc.TradeTimeStamp
ORDER BY
    TickerId";
        }


        public static FormattableString GetAllTradesWithSplitGroupedByMonthAndTicker__TradeTable(int userId, TickerTypes tickersType)
        {
            return $@"
;
WITH TradesBoundry AS (
	SELECT
		ISNULL(MIN(TradeTimeStamp), GETDATE()) AS MinDate
	   ,ISNULL(MAX(TradeTimeStamp), GETDATE()) AS MaxDate
	FROM [dbo].[Trade]
	WHERE
		UserIdentityId = {userId}
),
MonthSeries
AS
(SELECT
		DATEFROMPARTS(YEAR(TradesBoundry.MinDate), MONTH(TradesBoundry.MinDate), 1) AS MonthStart
	FROM TradesBoundry
	UNION ALL
	SELECT
		DATEADD(MONTH, 1, MonthStart)
	FROM MonthSeries
	JOIN TradesBoundry ON 1 = 1
	WHERE MonthStart < DATEFROMPARTS(YEAR(TradesBoundry.MaxDate), MONTH(TradesBoundry.MaxDate), 1)),
StockTickers
AS
(SELECT DISTINCT
		TickerId
	FROM [dbo].[Trade]
	JOIN [dbo].[EnumItem] ei
		ON ei.Id = [Trade].TickerId
	JOIN dbo.EnumItemType eit
		ON eit.Id = ei.EnumItemTypeId
		AND eit.Code = {tickersType.ToString()}
	WHERE
		UserIdentityId = {userId}),
TradesWithSplit
AS
(SELECT
		sth.TickerId
	   ,sth.TradeTimeStamp
	   ,sth.TradeSize
	   ,sth.TradeValue
	   ,COALESCE((SELECT
				EXP(SUM(LOG(ss.SplitCoefficient)))
			FROM [dbo].[StockSplit] ss
			WHERE ss.TickerId = sth.TickerId
			AND ss.SplitTimeStamp > sth.TradeTimeStamp)
		, 1) AS SplitAdjustment
	   ,YEAR(sth.TradeTimeStamp) AS TradeYear
	   ,MONTH(sth.TradeTimeStamp) AS TradeMonth
	FROM [dbo].[Trade] sth
	JOIN [dbo].[EnumItem] ei
		ON ei.Id = sth.TickerId
	JOIN dbo.EnumItemType eit
		ON eit.Id = ei.EnumItemTypeId
		AND eit.Code = {tickersType.ToString()}
	WHERE
		UserIdentityId = {userId}),
AggregatedTrades
AS
(SELECT
		st.TickerId
	   ,YEAR(ms.MonthStart) AS TradeYear
	   ,MONTH(ms.MonthStart) AS TradeMonth
	   ,ISNULL(SUM(tws.TradeSize * tws.SplitAdjustment), 0) AS TradeSize
	   ,ISNULL(SUM(tws.TradeValue), 0) AS TradeValue
	FROM MonthSeries ms
	CROSS JOIN StockTickers st
	LEFT JOIN TradesWithSplit tws
		ON st.TickerId = tws.TickerId
		AND tws.TradeYear = YEAR(ms.MonthStart)
		AND tws.TradeMonth = MONTH(ms.MonthStart)
	GROUP BY st.TickerId
			,YEAR(ms.MonthStart)
			,MONTH(ms.MonthStart)),
AccumulatedTrades
AS
(SELECT
		TickerId
	   ,TradeYear
	   ,TradeMonth
	   ,TradeSize
	   ,TradeValue
	   ,SUM(TradeSize) OVER (PARTITION BY TickerId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize
	FROM AggregatedTrades)
SELECT
	TickerId
   ,TradeYear
   ,TradeMonth
   ,TradeSize
   ,TradeValue
   ,AccumulatedTradeSize
FROM AccumulatedTrades
ORDER BY TickerId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0)";
        }
    }
}

public enum TickerTypes
{
    StockTradeTickers,
    CryptoTradeTickers
}
