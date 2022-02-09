using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBankAccountService : IBaseService<BankAccountModel>
    {
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(string userLogin, DateTime? toDate);

        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(int userId, DateTime? toDate);

        BankBalanceModel GetBankAccountBalanceToDate(int bankAccountId, DateTime? toDate);

        IEnumerable<BankAccountModel> GetAllBankAccounts(int userId);

        bool UserHasRightToBankAccount(int bankAccountId, int userId);
    }
}