﻿using Data;
using Data.DataModels;

namespace Repository
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        public BudgetRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}