using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for handling operations related to bank accounts.
    /// </summary>
    public interface IBankAccountService : IBaseService<BankAccountModel, BankAccount>
    {
        /// <summary>
        /// Gets the bank accounts balance to a specific date for a user by their login.
        /// </summary>
        /// <param name="userLogin">The user login.</param>
        /// <param name="toDate">The date to get the balance to.</param>
        /// <returns>A collection of bank balance models.</returns>
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(string userLogin, DateTime? toDate);

        /// <summary>
        /// Gets the bank accounts balance to a specific date for a user by their ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="toDate">The date to get the balance to.</param>
        /// <returns>A collection of bank balance models.</returns>
        IEnumerable<BankBalanceModel> GetBankAccountsBalanceToDate(int userId, DateTime? toDate);

        /// <summary>
        /// Gets the bank account balance to a specific date.
        /// </summary>
        /// <param name="bankAccountId">The bank account ID.</param>
        /// <param name="toDate">The date to get the balance to.</param>
        /// <returns>A bank balance model.</returns>
        BankBalanceModel GetBankAccountBalanceToDate(int bankAccountId, DateTime? toDate);

        /// <summary>
        /// Gets all bank accounts for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of bank account models.</returns>
        IEnumerable<BankAccountModel> GetAllBankAccounts(int userId);

        /// <summary>
        /// Checks if a user has the right to access a specific bank account.
        /// </summary>
        /// <param name="bankAccountId">The bank account ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>True if the user has the right to access the bank account, otherwise false.</returns>
        bool UserHasRightToBankAccount(int bankAccountId, int userId);
    }
}