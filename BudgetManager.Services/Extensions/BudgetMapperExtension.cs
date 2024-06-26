﻿using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Extensions
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
                Name = budget.Name,
                UserIdentityId = budget.UserIdentityId
            };
        }

        public static Budget MapToDataModel(this BudgetModel budgetModel, Budget budget)
        {
            budget.Amount = budgetModel.Amount;
            budget.DateFrom = budgetModel.DateFrom;
            budget.DateTo = budgetModel.DateTo;
            budget.Name = budgetModel.Name;
            budget.UserIdentityId = budgetModel.UserIdentityId;

            return budget;
        }
    }
}
