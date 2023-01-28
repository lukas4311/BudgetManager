using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        private const string bucket = "StockPrice";
        private const string organizationId = "8f46f33452affe4a";
        private readonly InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo;

        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper, InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo) : base(repository, mapper)
        {
            this.stockDataInfluxRepo = stockDataInfluxRepo;
        }

        public IEnumerable<StockTradeHistoryGetModel> GetAll(int userId) => this.repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => this.mapper.Map<StockTradeHistoryGetModel>(d));

        public bool UserHasRightToPayment(int stockTradeHistoruId, int userId)
            => this.repository.FindByCondition(a => a.Id == stockTradeHistoruId && a.UserIdentityId == userId).Count() == 1;

        public IEnumerable<StockPrice> GetStockPriceHistory(string ticker)
        {
            //this.stockDataInfluxRepo.
            this.stockDataInfluxRepo.GetAllData(new DataSourceIdentification(organizationId, bucket), f => f.Ticker == ticker);
            return null;
        }
    }
}