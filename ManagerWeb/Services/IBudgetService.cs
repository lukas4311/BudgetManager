﻿using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    public interface IBudgetService
    {
        void Add(BudgetModel budgetModel);
        IEnumerable<BudgetModel> Get();
        void Update(BudgetModel budgetModel);
    }
}