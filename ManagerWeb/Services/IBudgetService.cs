using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    internal interface IBudgetService
    {
        void Add(BudgetModel budgetModel);
        IEnumerable<BudgetModel> Get();
        BudgetModel Get(int id);
        void Update(BudgetModel budgetModel);
    }
}