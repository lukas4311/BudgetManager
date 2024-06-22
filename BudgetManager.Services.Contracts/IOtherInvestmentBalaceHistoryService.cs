using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for other investment balance history
    /// </summary>
    public interface IOtherInvestmentBalaceHistoryService : IBaseService<OtherInvestmentBalaceHistoryModel, OtherInvestmentBalaceHistory>
    {
    }
}