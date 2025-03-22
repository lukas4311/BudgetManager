using System;

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

        public static string GetAllComodityAccumulatedSizeAndValueInMonths()
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
        YEAR(cth.TradeTimeStamp) AS TradeYear,
        MONTH(cth.TradeTimeStamp) AS TradeMonth
    FROM
        [dbo].[ComodityTradeHistory] cth
    WHERE
      		cth.UserIdentityId = {0}
),
AggregatedTrades AS (
    SELECT
        ct.ComodityTypeId,
        YEAR(ms.MonthStart) AS TradeYear,
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
            t.TradeYear = YEAR(ms.MonthStart) AND 
            t.TradeMonth = MONTH(ms.MonthStart)
    GROUP BY
        ct.ComodityTypeId,
        YEAR(ms.MonthStart),
        MONTH(ms.MonthStart)
),
AccumulatedTrades AS (
    SELECT
        ComodityTypeId,
        TradeYear,
        TradeMonth,
        SUM(TradeSize) OVER (PARTITION BY ComodityTypeId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeSize,
        SUM(TradeValue) OVER (PARTITION BY ComodityTypeId ORDER BY TradeYear, TradeMonth) AS AccumulatedTradeValue
    FROM
        AggregatedTrades
)
SELECT
    ComodityTypeId,
    TradeYear,
    TradeMonth,
    AccumulatedTradeSize,
    AccumulatedTradeValue
FROM
    AccumulatedTrades
ORDER BY
    ComodityTypeId, TradeYear, TradeMonth
OPTION (MAXRECURSION 0)
                    ";
        }
    }
}
