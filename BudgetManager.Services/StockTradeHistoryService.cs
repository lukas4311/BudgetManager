using AutoMapper;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Enums;
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
        private const string BrokerStockTypeCode = "Stock";
        private const string BrokerProcessStateCode = "InProcess";
        private readonly IStockTradeHistoryRepository repository;
        private readonly IMapper mapper;
        private readonly InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo;
        private readonly IInfluxContext influxContext;
        private readonly IStockSplitService stockSplitService;
        private readonly IForexService forexService;
        private readonly ICurrencySymbolRepository currencySymbolRepository;
        private readonly IBrokerReportTypeRepository brokerReportTypeRepository;
        private readonly IBrokerReportToProcessStateRepository brokerReportToProcessStateRepository;
        private readonly IBrokerReportToProcessRepository brokerReportToProcessRepository;
        private readonly IDateTime dateTimeProvider;

        public StockTradeHistoryService(IStockTradeHistoryRepository repository, IMapper mapper,
            InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo, IInfluxContext influxContext,
            IStockSplitService stockSplitService, IForexService forexService, ICurrencySymbolRepository currencySymbolRepository,
            IBrokerReportTypeRepository brokerReportTypeRepository, IBrokerReportToProcessStateRepository brokerReportToProcessStateRepository,
            IBrokerReportToProcessRepository brokerReportToProcessRepository, IDateTime dateTimeProvider) : base(repository, mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.stockDataInfluxRepo = stockDataInfluxRepo;
            this.influxContext = influxContext;
            this.stockSplitService = stockSplitService;
            this.forexService = forexService;
            this.currencySymbolRepository = currencySymbolRepository;
            this.brokerReportTypeRepository = brokerReportTypeRepository;
            this.brokerReportToProcessStateRepository = brokerReportToProcessStateRepository;
            this.brokerReportToProcessRepository = brokerReportToProcessRepository;
            this.dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<StockTradeHistoryGetModel> GetAll(int userId)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d))
                .ToList();

            IEnumerable<StockSplitModel> splits = stockSplitService.GetAll();

            if (splits.Any())
                ApplySplitsToTrades(trades, splits);

            return trades;
        }

        public async Task<IEnumerable<StockTradeHistoryGetModel>> GetAll(int userId, ECurrencySymbol currencySymbol)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d))
                .ToList();

            IEnumerable<StockSplitModel> splits = this.stockSplitService.GetAll();

            if (splits.Any())
                ApplySplitsToTrades(trades, splits);

            IEnumerable<CurrencySymbol> currencySymbols = this.currencySymbolRepository.FindAll().ToList();

            for (int i = 0; i < trades.Count; i++)
            {
                StockTradeHistoryGetModel trade = trades[i];
                CurrencySymbol currencySymbolEntity = this.currencySymbolRepository.FindByCondition(a => a.Symbol == currencySymbol.ToString()).Single();
                double exhangeRate = await forexService.GetExchangeRate(trade.CurrencySymbol, currencySymbol.ToString(), trade.TradeTimeStamp);
                trade.TradeValue *= exhangeRate;
                trade.CurrencySymbol = currencySymbol.ToString();
                trade.CurrencySymbolId = currencySymbolEntity.Id;
            }

            return trades;
        }

        public IEnumerable<StockTradeHistoryGetModel> GetTradeHistory(int userId, string stockTicker)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Include(t => t.StockTicker)
                .Where(t => t.StockTicker.Ticker == stockTicker)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d))
                .ToList();

            if (trades.Any())
            {
                IEnumerable<StockSplitModel> splits = stockSplitService.Get(s => s.StockTickerId == trades[0].StockTickerId);

                if (splits.Any())
                    ApplySplitsToTrades(trades, splits);
            }

            return trades;
        }

        public bool UserHasRightToStockTradeHistory(int stockTradeHistoruId, int userId)
            => repository.FindByCondition(a => a.Id == stockTradeHistoruId && a.UserIdentityId == userId).Count() == 1;

        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker)
            => await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), new() { { "ticker", ticker } });

        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker, DateTime from)
            => await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), from, new() { { "ticker", ticker } });

        public async Task<StockPrice> GetStockPriceAtDate(string ticker, DateTime atDate)
            => (await stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), new DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "ticker", ticker } })).LastOrDefault();

        public async Task<IEnumerable<StockPrice>> GetStocksPriceAtDate(string[] tickers, DateTime date)
        {
            List<Task<IEnumerable<StockPrice>>> finPriceTasks = new();

            for (int i = 0; i < tickers.Length; i++)
            {
                Task<IEnumerable<StockPrice>> taskTicker = stockDataInfluxRepo.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucket), new DateTimeRange { From = date.AddDays(-5), To = date.AddDays(1) }, new() { { "ticker", tickers[i] } });
                finPriceTasks.Add(taskTicker);
            }

            IEnumerable<StockPrice>[] prices = await Task.WhenAll(finPriceTasks.ToArray());
            return prices.Where(m => m.Any()).Select(m => m.FirstOrDefault());
        }

        private void ApplySplitsToTrades(IEnumerable<StockTradeHistoryGetModel> trades, IEnumerable<StockSplitModel> splits)
        {
            foreach (StockTradeHistoryGetModel trade in trades)
            {
                double splitCoefficient = this.stockSplitService.GetAccumulatedCoefficient(splits.Where(c =>
                    c.SplitTimeStamp >= trade.TradeTimeStamp && c.StockTickerId == trade.StockTickerId));
                trade.TradeSizeAfterSplit = splitCoefficient * trade.TradeSize;
            }
        }

        public void StoreReportToProcess(byte[] brokerFileData, int userId)
        {
            string fileContentBase64 = Convert.ToBase64String(brokerFileData);

            int stockTypeId = this.brokerReportTypeRepository.FindByCondition(t => t.Code == BrokerStockTypeCode).Single().Id;
            int stockStateId = this.brokerReportToProcessStateRepository.FindByCondition(t => t.Code == BrokerProcessStateCode).Single().Id;

            BrokerReportToProcess brokerReport = new BrokerReportToProcess
            {
                BrokerReportToProcessStateId = stockStateId,
                BrokerReportTypeId = stockTypeId,
                FileContentBase64 = fileContentBase64,
                ImportedTime = this.dateTimeProvider.Now.DateTimeInstance,
                UserIdentityId = userId
            };

            this.brokerReportToProcessRepository.Create(brokerReport);
            this.brokerReportToProcessRepository.Save();
        }
    }
}