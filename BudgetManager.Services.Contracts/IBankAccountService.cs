using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBankAccountService
    {
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(DateTime? toDate);

        IEnumerable<BankAccountModel> GetAllBankAccounts();

        int AddBankAccount(BankAccountModel bankAccountViewModel);

        void UpdateBankAccount(BankAccountModel bankAccountViewModel);

        void DeleteBankAccount(int id);
    }
}