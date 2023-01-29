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
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        private const string bucket = "StockPrice";
        private const string organizationId = "f209a688c8dcfff3";
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

        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker) 
            => await this.stockDataInfluxRepo.GetAllData(new DataSourceIdentification(organizationId, bucket), new() { { "ticker", ticker } });
    }
}