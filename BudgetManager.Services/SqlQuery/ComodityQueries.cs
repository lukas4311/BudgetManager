using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class ComodityQueries
    {
        public static FormattableString GetAllComodityTradeSizeAndValue()
        {
            return $"""
                    SELECT
                        ComodityTypeId,
                        SUM(CASE WHEN TradeValue >= 0 THEN -TradeSize ELSE TradeSize END) AS TotalTradeSize,
                        SUM(TradeValue) AS TotalTradeValue
                    FROM
                        [dbo].[ComodityTradeHistory]
                    GROUP BY
                        ComodityTypeId
                    ORDER BY
                        ComodityTypeId
                    """;
        }

        public static FormattableString GetAllComodityAccumulatedSizeAndValueInMonths(DateTime from, DateTime to)
        {
            return $"""
                    WITH MonthSeries AS (
                        SELECT
                            DATEFROMPARTS(YEAR({from:s}), MONTH({from:s}), 1) AS MonthStart
                        UNION ALL
                        SELECT
                            DATEADD(MONTH, 1, MonthStart)
                        FROM
                            MonthSeries
                        WHERE
                            MonthStart < DATEFROMPARTS(YEAR({to:s}), MONTH({to:s}), 1)
                    ),
                    ComodityTypes AS (
                        SELECT DISTINCT ComodityTypeId
                        FROM [dbo].[ComodityTradeHistory]
                    ),
                    Trades AS (
                        SELECT
                            cth.ComodityTypeId,
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
                    """;
        }
    }
}
