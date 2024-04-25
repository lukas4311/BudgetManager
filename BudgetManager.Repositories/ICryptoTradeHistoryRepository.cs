using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <summary>
    /// Repository for history of crypto trades
    /// </summary>
    public interface ICryptoTradeHistoryRepository : IRepository<CryptoTradeHistory>
    { }
}