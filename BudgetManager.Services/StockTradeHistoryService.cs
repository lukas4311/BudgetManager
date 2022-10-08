using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public IEnumerable<StockTradeHistoryModel> GetAll(int userId)
            => this.repository.FindByCondition(i => i.UserIdentityId == userId).Select(i => this.mapper.Map<StockTradeHistoryModel>(i));

        public bool UserHasRightToPayment(int stockTradeHistoruId, int userId)
            => this.repository.FindByCondition(a => a.Id == stockTradeHistoruId && a.UserIdentityId == userId).Count() == 1;
    }
}
