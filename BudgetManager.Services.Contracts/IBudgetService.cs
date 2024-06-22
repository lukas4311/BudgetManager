using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for handling budget-related operations.
    /// </summary>
    public interface IBudgetService : IBaseService<BudgetModel, Budget>
    {
        /// <summary>
        /// Retrieves all budgets.
        /// </summary>
        /// <returns>A collection of <see cref="BudgetModel"/> objects.</returns>
        IEnumerable<BudgetModel> Get();

        /// <summary>
        /// Retrieves budgets for a specific user within a date range.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fromDate">The start date of the range.</param>
        /// <param name="toDate">The end date of the range. If null, defaults to <see cref="DateTime.MaxValue"/>.</param>
        /// <returns>A collection of <see cref="BudgetModel"/> objects within the specified date range.</returns>
        IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate);

        /// <summary>
        /// Retrieves actual budgets for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A collection of actual <see cref="BudgetModel"/> objects for the specified user.</returns>
        IEnumerable<BudgetModel> GetActual(int userId);

        /// <summary>
        /// Retrieves budgets for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A collection of <see cref="BudgetModel"/> objects for the specified user.</returns>
        IEnumerable<BudgetModel> GetByUserId(int userId);

        /// <summary>
        /// Checks if a user has rights to a specific budget.
        /// </summary>
        /// <param name="budgetId">The budget identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if the user has rights to the budget; otherwise, <c>false</c>.</returns>
        bool UserHasRightToBudget(int budgetId, int userId);
    }
}