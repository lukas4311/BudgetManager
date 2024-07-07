using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class PaymentQueries
    {
        public static FormattableString GetAllBankAccountsSeparatelyCurrentBalance()
        {
            return $"""

                    SELECT
                        ba.Id AS BankAccountId,
                        ba.Code AS BankAccountCode,
                        ba.OpeningBalance,
                        ISNULL(SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END), 0) AS TotalPayments,
                        ba.OpeningBalance + ISNULL(SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END), 0) AS CurrentBalance
                    FROM
                        BankAccount ba
                    LEFT JOIN
                        Payment p ON ba.Id = p.BankAccountId
                    LEFT JOIN
                        PaymentType pt ON p.PaymentTypeId = pt.Id
                    GROUP BY
                        ba.Id, ba.Code, ba.OpeningBalance
                    ORDER BY
                        ba.Id

                    """;
        }

        public static FormattableString GetAllBankAccountsTotalPaymentsGroupedByMonth()
        {
            return $"""

                    WITH MonthlyPayments AS (
                        SELECT
                            YEAR(p.Date) AS Year,
                            MONTH(p.Date) AS Month,
                            SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END) AS TotalPayments
                        FROM
                            Payment p
                        LEFT JOIN
                            PaymentType pt ON p.PaymentTypeId = pt.Id
                        GROUP BY
                            YEAR(p.Date), MONTH(p.Date)
                    ),
                    CumulativePayments AS (
                        SELECT
                            Year,
                            Month,
                            SUM(TotalPayments) OVER (ORDER BY Year, Month) AS CumulativeTotalPayments
                        FROM
                            MonthlyPayments
                    ),
                    TotalOpeningBalance AS (
                        SELECT
                            SUM(ba.OpeningBalance) AS TotalOpeningBalance
                        FROM
                            BankAccount ba
                    )
                    SELECT
                        cp.Year,
                        cp.Month,
                        tob.TotalOpeningBalance,
                        tob.TotalOpeningBalance + cp.CumulativeTotalPayments AS CurrentBalance
                    FROM
                        CumulativePayments cp
                    CROSS JOIN
                        TotalOpeningBalance tob
                    ORDER BY
                        cp.Year, cp.Month
                    """;
        }

        public static FormattableString GetAllBankAccountsSeparatelyTotalPaymentsGroupedByMonth()
        {
            return $"""

                    WITH MonthlyPayments AS (
                        SELECT
                            ba.Id AS BankAccountId,
                            YEAR(p.Date) AS Year,
                            MONTH(p.Date) AS Month,
                            SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END) AS TotalPayments
                        FROM
                            Payment p
                        LEFT JOIN
                            PaymentType pt ON p.PaymentTypeId = pt.Id
                        LEFT JOIN
                            BankAccount ba ON p.BankAccountId = ba.Id
                        GROUP BY
                            ba.Id, YEAR(p.Date), MONTH(p.Date)
                    ),
                    CumulativePayments AS (
                        SELECT
                            mp.BankAccountId,
                            mp.Year,
                            mp.Month,
                            SUM(mp.TotalPayments) OVER (PARTITION BY mp.BankAccountId ORDER BY mp.Year, mp.Month) AS CumulativeTotalPayments
                        FROM
                            MonthlyPayments mp
                    ),
                    Balances AS (
                        SELECT
                            ba.Id AS BankAccountId,
                            ba.Code AS BankAccountCode,
                            ba.OpeningBalance,
                            cp.Year,
                            cp.Month,
                            ba.OpeningBalance + cp.CumulativeTotalPayments AS CurrentBalance
                        FROM
                            BankAccount ba
                        LEFT JOIN
                            CumulativePayments cp ON ba.Id = cp.BankAccountId
                    )
                    SELECT
                        Year,
                        Month,
                        BankAccountId,
                        BankAccountCode,
                        OpeningBalance,
                        CurrentBalance
                    FROM
                        Balances
                    ORDER BY
                        Year, Month, BankAccountId

                    """;
        }

        public static FormattableString GetAllPaymentsInMonthsAccumulatedSeparatelyBankAccounts(DateTime from, DateTime to)
        {
            return $"""

                    ;WITH MonthSeries AS (
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
                    AllMonths AS (
                        SELECT
                            ba.Id AS BankAccountId,
                            ms.MonthStart,
                            YEAR(ms.MonthStart) AS Year,
                            MONTH(ms.MonthStart) AS Month
                        FROM
                            BankAccount ba
                        CROSS JOIN
                            MonthSeries ms
                    ),
                    MonthlyPayments AS (
                        SELECT
                            am.BankAccountId,
                            am.Year,
                            am.Month,
                            ISNULL(SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END), 0) AS TotalPayments
                        FROM
                            AllMonths am
                        LEFT JOIN
                            Payment p ON am.BankAccountId = p.BankAccountId
                                       AND YEAR(p.Date) = am.Year
                                       AND MONTH(p.Date) = am.Month
                        LEFT JOIN
                            PaymentType pt ON p.PaymentTypeId = pt.Id
                        GROUP BY
                            am.BankAccountId, am.Year, am.Month
                    ),
                    AccumulatedBalances AS (
                        SELECT
                            mp.BankAccountId,
                            mp.Year,
                            mp.Month,
                            mp.TotalPayments,
                            SUM(mp.TotalPayments) OVER (PARTITION BY mp.BankAccountId ORDER BY mp.Year, mp.Month) AS AccumulatedTotalPayments
                        FROM
                            MonthlyPayments mp
                    ),
                    BankAccountBalances AS (
                        SELECT
                            ba.Id AS BankAccountId,
                            ba.Code AS BankAccountCode,
                            ab.Year,
                            ab.Month,
                            ab.TotalPayments,
                            ab.AccumulatedTotalPayments,
                            ba.OpeningBalance + ab.AccumulatedTotalPayments AS CurrentBalance
                        FROM
                            AccumulatedBalances ab
                        JOIN
                            BankAccount ba ON ab.BankAccountId = ba.Id
                    )
                    SELECT
                        bab.BankAccountId,
                        bab.BankAccountCode,
                        bab.Year,
                        bab.Month,
                        bab.TotalPayments,
                        bab.AccumulatedTotalPayments,
                        bab.CurrentBalance
                    FROM
                        BankAccountBalances bab
                    ORDER BY
                        bab.BankAccountId, bab.Year, bab.Month
                    """;
        }

        public static FormattableString GetAllPaymentsInMonthsAccumulatedBankAccountsTogether(DateTime from, DateTime to)
        {
            return $"""
                    ;WITH MonthSeries AS (
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
                    AllMonths AS (
                        SELECT 
                            ms.MonthStart,
                            YEAR(ms.MonthStart) AS Year,
                            MONTH(ms.MonthStart) AS Month
                        FROM 
                            MonthSeries ms
                    ),
                    MonthlyPayments AS (
                        SELECT 
                            am.Year,
                            am.Month,
                            ISNULL(SUM(CASE WHEN pt.Code = 'Expense' THEN -p.Amount ELSE p.Amount END), 0) AS TotalPayments
                        FROM 
                            AllMonths am
                        LEFT JOIN 
                            Payment p ON YEAR(p.Date) = am.Year 
                                       AND MONTH(p.Date) = am.Month
                        LEFT JOIN 
                            PaymentType pt ON p.PaymentTypeId = pt.Id
                        GROUP BY 
                            am.Year, am.Month
                    ),
                    AccumulatedBalances AS (
                        SELECT 
                            mp.Year,
                            mp.Month,
                            mp.TotalPayments,
                            SUM(mp.TotalPayments) OVER (ORDER BY mp.Year, mp.Month) AS AccumulatedTotalPayments
                        FROM 
                            MonthlyPayments mp
                    ),
                    TotalOpeningBalance AS (
                        SELECT 
                            SUM(ba.OpeningBalance) AS TotalOpeningBalance
                        FROM 
                            BankAccount ba
                    ),
                    FinalBalances AS (
                        SELECT
                            ab.Year,
                            ab.Month,
                            tob.TotalOpeningBalance,
                            ab.TotalPayments,
                            ab.AccumulatedTotalPayments,
                            tob.TotalOpeningBalance + ab.AccumulatedTotalPayments AS CurrentBalance
                        FROM 
                            AccumulatedBalances ab
                        CROSS JOIN 
                            TotalOpeningBalance tob
                    )
                    SELECT 
                        fb.Year,
                        fb.Month,
                        fb.TotalOpeningBalance,
                        fb.TotalPayments,
                        fb.AccumulatedTotalPayments,
                        fb.CurrentBalance
                    FROM 
                        FinalBalances fb
                    ORDER BY 
                        fb.Year, fb.Month
                    """;
        }
    }
}