using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for other investment balance history
    /// </summary>
    public interface IOtherInvestmentBalaceHistoryService : IBaseService<OtherInvestmentBalaceHistoryModel, OtherInvestmentBalaceHistory, IRepository<OtherInvestmentBalaceHistory>>
    {
    }
}