using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBudgetService : IBaseService<BudgetModel, Budget>
    {
        IEnumerable<BudgetModel> Get();

        IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate);

        IEnumerable<BudgetModel> GetActual(int userId);

        IEnumerable<BudgetModel> GetByUserId(int userId);

        bool UserHasRightToBudget(int budgetId, int userId);
    }
}