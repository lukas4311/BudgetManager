using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBudgetService : IBaseService<BudgetModel>
    {
        //void Add(BudgetModel budgetModel);

        //void Delete(int id);

        //void Update(BudgetModel budgetModel);

        IEnumerable<BudgetModel> Get();

        //BudgetModel Get(int id);

        IEnumerable<BudgetModel> Get(int userId, DateTime fromDate, DateTime? toDate);

        IEnumerable<BudgetModel> GetActual(int userId);

        IEnumerable<BudgetModel> GetByUserId(int userId);

        bool UserHasRightToBudget(int budgetId, int userId);
    }
}