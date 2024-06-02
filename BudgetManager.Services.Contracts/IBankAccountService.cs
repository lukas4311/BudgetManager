using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Bank account service
    /// </summary>
    public interface IBankAccountService : IBaseService<BankAccountModel, BankAccount>
    {
        /// <summary>
        /// Calculate current bank account balance
        /// </summary>
        /// <param name="userLogin">User login</param>
        /// <param name="toDate">Calculation for this date</param>
        /// <returns>Balance model with current and opening balance</returns>
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(string userLogin, DateTime? toDate);

        /// <summary>
        /// Calculate current bank account balance
        /// </summary>
        /// <param name="userLogin">User login</param>
        /// <param name="toDate">Calculation for this date</param>
        /// <returns>Balance model with current and opening balance</returns>
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(int userId, DateTime? toDate);

        /// <summary>
        /// Calculate specific bank account balace for specific day
        /// </summary>
        /// <param name="bankAccountId">Bank account id</param>
        /// <param name="toDate">Specific data for calculation</param>
        /// <returns>Bank balance modle for specific date</returns>
        BankBalanceModel GetBankAccountBalanceToDate(int bankAccountId, DateTime? toDate);

        /// <summary>
        /// Get all bank accounts
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Models with bank accounts info</returns>
        IEnumerable<BankAccountModel> GetAllBankAccounts(int userId);

        /// <summary>
        /// Check if user has rights for specific bank account
        /// </summary>
        /// <param name="bankAccountId">Bank account id</param>
        /// <param name="userId">User id</param>
        /// <returns><see langword="true"/> if user has rights</returns>
        bool UserHasRightToBankAccount(int bankAccountId, int userId);
    }
}