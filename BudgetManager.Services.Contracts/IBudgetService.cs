using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IBudgetService
    {
        IEnumerable<BudgetModel> Get();

        BudgetModel Get(int id);

        void Add(BudgetModel budgetModel);

        void Update(BudgetModel budgetModel);

        void Delete(int id);

        IEnumerable<BudgetModel> GetActual();

        IEnumerable<BudgetModel> Get(DateTime fromDate, DateTime? toDate);
    }
}