using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IStockTradeHistoryRepository>, IStockTradeHistoryService
    {
        private const string bucket = "StockPrice";
        private readonly InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo;
        private readonly IInfluxContext influxContext;

        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper, InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo, IInfluxContext influxContext) : base(repository, mapper)
        {
            this.stockDataInfluxRepo = stockDataInfluxRepo;
            this.influxContext = influxContext;
        }

        public IEnumerable<StockTradeHistoryGetModel> GetAll(int userId) => repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d));

        public IEnumerable<StockTradeHistoryGetModel> GetTradeHistory(int userId, string stockTicker)
        {
            return repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Include(t => t.StockTicker)
                .Where(t => t.StockTicker.Ticker == stockTicker)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d));
        }

        public bool UserHasRightToStockTradeHistory(int stockTradeHistoruId, int userId)
            => repository.FindByCondition(a => a.Id == stockTradeHistoruId && a.UserIdentityId == userId).Count() == 1;

        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker) 
            => await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), new() { { "ticker", ticker } });

        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker, DateTime from)
            => await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), from, new() { { "ticker", ticker } });

        public async Task<StockPrice> GetStockPriceAtDate(string ticker, DateTime atDate)
            => (await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), new DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "ticker", ticker } })).LastOrDefault();
    }
}