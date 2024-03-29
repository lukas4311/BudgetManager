﻿using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class OtherInvestmentTagRepository : Repository<OtherInvestmentTag>, IOtherInvestmentTagRepository
    {
        public OtherInvestmentTagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
