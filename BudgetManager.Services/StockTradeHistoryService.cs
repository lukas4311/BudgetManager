using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public IEnumerable<StockTradeHistoryGetModel> GetAll(int userId) => this.repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => this.mapper.Map<StockTradeHistoryGetModel>(d));

        public bool UserHasRightToPayment(int stockTradeHistoruId, int userId)
            => this.repository.FindByCondition(a => a.Id == stockTradeHistoruId && a.UserIdentityId == userId).Count() == 1;
    }
}