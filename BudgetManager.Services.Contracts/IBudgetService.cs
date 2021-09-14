using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBudgetService
    {
        void Add(BudgetModel budgetModel);

        void Delete(int id);

        IEnumerable<BudgetModel> Get();

        BudgetModel Get(int id);

        IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate);

        IEnumerable<BudgetModel> GetActual(int userId);

        IEnumerable<BudgetModel> GetByUserId(int userId);

        void Update(BudgetModel budgetModel);

        bool UserHasRightToBudget(int budgetId, int userId);
    }
}