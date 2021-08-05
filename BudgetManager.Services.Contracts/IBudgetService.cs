using BudgetManager.ManagerWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IBudgetService
    {
        void Add(BudgetModel budgetModel);
        IEnumerable<BudgetModel> Get();
        BudgetModel Get(int id);
        void Update(BudgetModel budgetModel);
        void Delete(int id);
        IEnumerable<BudgetModel> GetActual();

        IEnumerable<BudgetModel> Get(DateTime fromDate, DateTime? toDate);
    }
}