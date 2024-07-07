using System;

namespace BudgetManager.Services.SqlQuery
{
    internal static class PaymentQueries
    {
        public static FormattableString GetAllBankAccountsSeparatelyCurrentBalance()
        {
            return $@"
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
";
        }

        public static FormattableString GetAllBankAccountsTotalPayments()
        {
            return $@"
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
ORDER BY 
    Year, Month;
";
        }

        public static FormattableString GetAllBankAccountsSeparatelyTotalPayments()
        {
            return $@"
SELECT 
    p.BankAccountId,
    YEAR(p.Date) AS Year,
    MONTH(p.Date) AS Month,
    SUM(p.Amount) AS TotalPayments
FROM 
    Payment p
GROUP BY 
    p.BankAccountId, YEAR(p.Date), MONTH(p.Date)
ORDER BY 
    p.BankAccountId, Year, Month;
";
        }

    }
}
