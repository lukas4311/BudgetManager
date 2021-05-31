using ManagerWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace ManagerWeb.Services
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