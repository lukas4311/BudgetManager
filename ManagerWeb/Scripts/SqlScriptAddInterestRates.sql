
/****** Script for SelectTopNRows command from SSMS  ******/

WITH BankAccountBalance
     AS (SELECT ba.Id AS BankAccountId, 
                SUM(Src.AmountWithNegatives) + ba.OpeningBalance AS Balance
         FROM
         (
             SELECT ba.Id,
                    CASE
                        WHEN pt.Code = 'Expense'
                        THEN -p.Amount
                        ELSE p.Amount
                    END AS AmountWithNegatives
             FROM dbo.BankAccount ba
                  JOIN dbo.Payment p ON ba.Id = p.BankAccountId
                  JOIN dbo.PaymentType pt ON p.PaymentTypeId = pt.Id
         ) AS Src
         JOIN dbo.BankAccount ba ON ba.id = src.Id
         GROUP BY ba.Id, 
                  ba.OpeningBalance)
     SELECT Src.BankAccountId, 
            SUM(Src.InterestsToAdd)
     FROM
     (
         SELECT Ba.Id AS BankAccountId, 
                I.[PayoutDate], 
                BAB.Balance,
                CASE
                    WHEN BAB.Balance - I.RangeFrom > 0
                    THEN(BAB.Balance - I.RangeFrom) * (I.[Value] / 100)
                    ELSE 0
                END AS InterestsToAdd
         FROM [dbo].[InterestRate] AS I
              JOIN dbo.BankAccount AS ba ON I.BankAccountId = ba.Id
              JOIN BankAccountBalance AS BAB ON BAB.BankAccountId = ba.Id
     ) AS Src
     GROUP BY Src.BankAccountId