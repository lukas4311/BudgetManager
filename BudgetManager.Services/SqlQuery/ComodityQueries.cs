﻿using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class ComodityQueries
    {
        public static string GetAllComodityTradeSizeAndValue()
        {
            return @"
                    SELECT
                        ComodityTypeId,
                        SUM(CASE WHEN TradeValue >= 0 THEN -TradeSize ELSE TradeSize END) AS TotalTradeSize,
                        SUM(TradeValue) AS TotalTradeValue
                    FROM
                        [dbo].[ComodityTradeHistory]
                    WHERE
                        UserIdentityId = {0}
                    GROUP BY
                        ComodityTypeId
                    ORDER BY
                        ComodityTypeId
                    ";
        }

        public static string GetAllComodityTradesGroupedByTickerAndTradeDate__TradeTable()
        {
            return @"
;
WITH TradesBoundry AS (
	SELECT
		ISNULL(MIN(TradeTimeStamp), GETDATE()) AS MinDate
	   ,ISNULL(MAX(TradeTimeStamp), GETDATE()) AS MaxDate
	FROM [dbo].[ComodityTradeHistory]
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
	WHERE MonthStart < DATEFROMPARTS(YEAR(TradesBoundry.MaxDate), MONTH(TradesBoundry.MaxDate), 1)
),
ComodityTypes AS (
    SELECT DISTINCT ComodityTypeId
    FROM [dbo].[ComodityTradeHistory]
),
Trades AS (
    SELECT
        cth.ComodityTypeId,
        cth.TradeTimeStamp,
        cth.TradeSize AS AdjustedTradeSize,
        cth.TradeValue,
        YEAR(cth.TradeTimeStamp) AS Year,
        MONTH(cth.TradeTimeStamp) AS TradeMonth
    FROM
        [dbo].[ComodityTradeHistory] cth
    WHERE
      		cth.UserIdentityId = {0}
),
AggregatedTrades AS (
    SELECT
        ct.ComodityTypeId,
        YEAR(ms.MonthStart) AS Year,
        MONTH(ms.MonthStart) AS TradeMonth,
        ISNULL(SUM(t.AdjustedTradeSize), 0) AS TradeSize,
        ISNULL(SUM(t.TradeValue), 0) AS TradeValue
    FROM
        MonthSeries ms
    CROSS JOIN
        ComodityTypes ct
    LEFT JOIN
        Trades t ON 
            ct.ComodityTypeId = t.ComodityTypeId AND
            t.Year = YEAR(ms.MonthStart) AND 
            t.TradeMonth = MONTH(ms.MonthStart)
    GROUP BY
        ct.ComodityTypeId,
        YEAR(ms.MonthStart),
        MONTH(ms.MonthStart)
),
AccumulatedTrades AS (
    SELECT
        ComodityTypeId,
        Year,
        TradeMonth,
        SUM(TradeSize) OVER (PARTITION BY ComodityTypeId ORDER BY Year, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY ComodityTypeId ORDER BY Year, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    ComodityTypeId,
    Year,
    TradeMonth,
    AccumulatedTradeSize,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    ComodityTypeId, Year, TradeMonth
OPTION (MAXRECURSION 0)
                    ";
        }

        public static string GetAllComodityAccumulatedSizeAndValueInMonths()
        {
            return @"
WITH Trades AS (
    SELECT
        cth.ComodityTypeId,
        cth.CurrencySymbolId,
        CAST(cth.TradeTimeStamp AS DATE) AS TradeDate,
        SUM(cth.TradeSize) AS TradeSize,
        SUM(cth.TradeValue) AS TradeValue
    FROM
        [dbo].[ComodityTradeHistory] cth
    WHERE
        cth.UserIdentityId = {0}
    GROUP BY
        cth.ComodityTypeId,
        cth.CurrencySymbolId,
        CAST(cth.TradeTimeStamp AS DATE)
),
AccumulatedTrades AS (
    SELECT
        ComodityTypeId,
        CurrencySymbolId,
        TradeDate,
        TradeSize,
        TradeValue,
        SUM(TradeSize) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeDate) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY ComodityTypeId, CurrencySymbolId ORDER BY TradeDate) AS AccumulatedTradeValue
    FROM
        Trades
)
SELECT
    ComodityTypeId,
    CurrencySymbolId,
    TradeDate,
    TradeSize,
    AccumulatedTradeSize,
    TradeValue,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    ComodityTypeId, 
    CurrencySymbolId,
    TradeDate
                    ";
        }
    }
}
