using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
