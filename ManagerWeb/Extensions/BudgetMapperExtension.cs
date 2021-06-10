using BudgetManager.Data.DataModels;
using BudgetManager.ManagerWeb.Models.DTOs;

namespace BudgetManager.ManagerWeb.Extensions
{
    internal static class BudgetMapperExtension
    {
        public static BudgetModel MapToViewModel(this Budget budget)
        {
            return new BudgetModel
            {
                Amount = budget.Amount,
                DateFrom = budget.DateFrom,
                DateTo = budget.DateTo,
                Id = budget.Id,
                Name = budget.Name
            };
        }

        public static Budget MapToDataModel(this BudgetModel budgetModel, Budget budget)
        {
            budget.Amount = budgetModel.Amount;
            budget.DateFrom = budgetModel.DateFrom;
            budget.DateTo = budgetModel.DateTo;
            budget.Name = budgetModel.Name;

            return budget;
        }
    }
}
