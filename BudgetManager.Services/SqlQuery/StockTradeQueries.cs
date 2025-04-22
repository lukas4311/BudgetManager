using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class StockTradeQueries
    {
        public static string GetAllTradesGroupedByTickerAndTradeDate__TradeTable()
        {
            return @"
;WITH TradesWithSplit AS (
    SELECT
        sth.TickerId,
        sth.TradeTimeStamp,
        sth.TradeSize,
        sth.TradeValue,
		sth.TradeCurrencySymbolId,
        COALESCE((
            SELECT EXP(SUM(LOG(ss.SplitCoefficient)))
            FROM [dbo].[StockSplit] ss
            WHERE ss.TickerId = sth.TickerId
              AND ss.SplitTimeStamp > sth.TradeTimeStamp
        ), 1) AS SplitAdjustment,
		ei.Code AS TickerCode,
		cei.Code AS CurrencyCode
    FROM
        [dbo].[Trade] sth
	JOIN [dbo].[EnumItem] ei ON
		ei.Id = sth.TickerId
	JOIN dbo.EnumItemType eit ON
		eit.Id = ei.EnumItemTypeId
		AND eit.Code = {1}
	JOIN dbo.EnumItem AS cei ON
		cei.Id = sth.TradeCurrencySymbolId
    WHERE
		sth.UserIdentityId = {0}
),
AdjustedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TradeSize * SplitAdjustment AS AdjustedTradeSize,
        TradeValue,
		TradeCurrencySymbolId,
		TickerCode,
		CurrencyCode
    FROM
        TradesWithSplit
),
AggregatedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        ISNULL(SUM(AdjustedTradeSize), 0) AS TotalTradeSize,
        ISNULL(SUM(TradeValue), 0) AS TotalTradeValue,
		MIN(TradeCurrencySymbolId) AS TradeCurrencySymbolId,
		MIN(CurrencyCode) AS CurrencyCode,
		TickerCode
    FROM
        AdjustedTrades
    GROUP BY
        TickerId,
		TickerCode,
        TradeTimeStamp
),
AccumulatedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TotalTradeSize,
        TotalTradeValue,
		TradeCurrencySymbolId,
        SUM(TotalTradeSize) OVER (PARTITION BY TickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize,
		TickerCode,
		CurrencyCode
    FROM
        AggregatedTrades
)
SELECT
    TickerId,
    TradeTimeStamp,
    TotalTradeSize,
    TotalTradeValue,
	TradeCurrencySymbolId,
    AccumulatedTradeSize,
	TickerCode,
	CurrencyCode
FROM
    AccumulatedTrades
ORDER BY
    TickerId, TradeTimeStamp
";
        }

        public static string GetAllTradesGroupedByTicker__TradeTable()
        {
            return @"
;WITH TradesWithSplit AS (
    SELECT
        sth.TickerId,
        sth.TradeTimeStamp,
        sth.TradeSize,
        sth.TradeValue,
		sth.TradeCurrencySymbolId,
        COALESCE((
            SELECT EXP(SUM(LOG(ss.SplitCoefficient)))
            FROM [dbo].[StockSplit] ss
            WHERE ss.TickerId = sth.TickerId
              AND ss.SplitTimeStamp > sth.TradeTimeStamp
        ), 1) AS SplitAdjustment,
		ei.Code AS TickerCode,
		cei.Code AS CurrencyCode
    FROM
        [dbo].[Trade] sth
	JOIN [dbo].[EnumItem] ei ON
		ei.Id = sth.TickerId
	JOIN dbo.EnumItemType eit ON
		eit.Id = ei.EnumItemTypeId
		AND eit.Code = {1}
    JOIN dbo.EnumItem AS cei ON
		cei.Id = sth.TradeCurrencySymbolId
    WHERE
		sth.UserIdentityId = {0}
),
AdjustedTrades AS (
    SELECT
        TickerId,
        TradeTimeStamp,
        TradeSize * SplitAdjustment AS AdjustedTradeSize,
        TradeValue,
		TradeCurrencySymbolId,
		TickerCode,
		CurrencyCode
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
        SUM(AdjustedTradeSize) OVER (PARTITION BY TickerId ORDER BY TradeTimeStamp) AS AccumulatedTradeSize,
		TradeCurrencySymbolId,
		TickerCode,
		CurrencyCode
    FROM
        AdjustedTrades
)
SELECT
    AT.TickerId,
    TotalTradeSize,
    TotalTradeValue,
    AccumulatedTradeSize,
	TradeCurrencySymbolId,
	TickerCode,
	CurrencyCode
FROM
    AggregatedTrades AS AT
JOIN
    (SELECT TickerId, MAX(TradeTimeStamp) AS LastTradeTimeStamp
     FROM AdjustedTrades
     GROUP BY TickerId) AS lastTrade
ON
    AT.TickerId = lastTrade.TickerId
JOIN AccumulatedTrades acc ON
    lastTrade.TickerId = acc.TickerId
    AND lastTrade.LastTradeTimeStamp = acc.TradeTimeStamp
ORDER BY
    TickerId
";
        }


        public static string GetAllTradesWithSplitGroupedByMonthAndTicker__TradeTable()
        {
            return @"
;
WITH TradesBoundry AS (
	SELECT
		ISNULL(MIN(TradeTimeStamp), GETDATE()) AS MinDate
	   ,ISNULL(MAX(TradeTimeStamp), GETDATE()) AS MaxDate
	FROM [dbo].[Trade]
	WHERE
		UserIdentityId = {0}
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
		AND eit.Code = {1}
	WHERE
		UserIdentityId = {0}),
TradesTickerWIthCurrency AS (
	SELECT
		sth.TickerId
		,MIN(sth.TradeCurrencySymbolId) AS TradeCurrencySymbolId
	FROM 
		[dbo].[Trade] sth
	GROUP BY
		sth.TickerId
),
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
		AND eit.Code = {1}
	WHERE
		UserIdentityId = {0}),
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
		agt.TickerId
	   ,TradeYear
	   ,TradeMonth
	   ,TradeSize
	   ,TradeValue
	   ,SUM(TradeSize) OVER (PARTITION BY agt.TickerId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize
	   ,ttc.TradeCurrencySymbolId
	   ,sei.Code AS TickerCode
	   ,cei.Code AS CurrencyCode
	FROM AggregatedTrades agt
	LEFT JOIN TradesTickerWIthCurrency ttc ON
		ttc.TickerId = agt.TickerId
	LEFT JOIN dbo.EnumItem AS sei ON
		sei.Id = agt.TickerId
	LEFT JOIN dbo.EnumItem AS cei ON
		cei.Id = ttc.TradeCurrencySymbolId
)
SELECT
	TickerId
   ,TradeYear
   ,TradeMonth
   ,TradeSize
   ,TradeValue
   ,AccumulatedTradeSize
   ,TradeCurrencySymbolId
   ,TickerCode
   ,CurrencyCode
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
