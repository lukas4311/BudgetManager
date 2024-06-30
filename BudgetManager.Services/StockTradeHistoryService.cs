using AutoMapper;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Enums;
using Infl = BudgetManager.InfluxDbData;
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
    /// <inheritdoc/>
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, StockTradeHistory, IRepository<StockTradeHistory>>, IStockTradeHistoryService
    {
        private const string bucket = "StockPrice";
        private const string BrokerStockTypeCode = "Stock";
        private const string BrokerProcessStateCode = "InProcess";
        private readonly IRepository<StockTradeHistory> repository;
        private readonly IMapper mapper;
        private readonly InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo;
        private readonly Infl.IInfluxContext influxContext;
        private readonly IStockSplitService stockSplitService;
        private readonly IForexService forexService;
        private readonly IRepository<CurrencySymbol> currencySymbolRepository;
        private readonly IRepository<BrokerReportType> brokerReportTypeRepository;
        private readonly IRepository<BrokerReportToProcessState> brokerReportToProcessStateRepository;
        private readonly IRepository<BrokerReportToProcess> brokerReportToProcessRepository;
        private readonly IDateTime dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTradeHistoryService"/> class.
        /// </summary>
        /// <param name="repository">The repository for stock trade history.</param>
        /// <param name="mapper">The mapper for mapping between models.</param>
        /// <param name="stockDataInfluxRepo">The repository for stock price data.</param>
        /// <param name="influxContext">The context for InfluxDB.</param>
        /// <param name="stockSplitService">The service for stock splits.</param>
        /// <param name="forexService">The service for forex operations.</param>
        /// <param name="currencySymbolRepository">The repository for currency symbols.</param>
        /// <param name="brokerReportTypeRepository">The repository for broker report types.</param>
        /// <param name="brokerReportToProcessStateRepository">The repository for broker report process states.</param>
        /// <param name="brokerReportToProcessRepository">The repository for broker reports to process.</param>
        /// <param name="dateTimeProvider">The provider for date and time.</param>
        public StockTradeHistoryService(IRepository<StockTradeHistory> repository, IMapper mapper,
            InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo, Infl.IInfluxContext influxContext,
            IStockSplitService stockSplitService, IForexService forexService, IRepository<CurrencySymbol> currencySymbolRepository,
            IRepository<BrokerReportType> brokerReportTypeRepository, IRepository<BrokerReportToProcessState> brokerReportToProcessStateRepository,
            IRepository<BrokerReportToProcess> brokerReportToProcessRepository, IDateTime dateTimeProvider) : base(repository, mapper)
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IEnumerable<StockTradeHistoryGetModel>> GetAll(int userId, ECurrencySymbol currencySymbol)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d))
                .ToList();

            IEnumerable<StockSplitModel> splits = stockSplitService.GetAll();

            if (splits.Any())
                ApplySplitsToTrades(trades, splits);

            CurrencySymbol currencySymbolEntity = currencySymbolRepository.FindByCondition(a => a.Symbol == currencySymbol.ToString()).Single();

            for (int i = 0; i < trades.Count; i++)
            {
                StockTradeHistoryGetModel trade = trades[i];
                double exchangeRate = await forexService.GetExchangeRate(trade.CurrencySymbol, currencySymbol.ToString(), trade.TradeTimeStamp);
                trade.TradeValue *= exchangeRate;
                trade.CurrencySymbol = currencySymbol.ToString();
                trade.CurrencySymbolId = currencySymbolEntity.Id;
            }

            return trades;
        }

        /// <inheritdoc/>
        public IEnumerable<StockTradeHistoryGetModel> GetTradeHistory(int userId, string stockTicker)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.CurrencySymbol)
                .Include(t => t.StockTicker)
                .Where(t => t.StockTicker.Ticker == stockTicker)
                .Select(d => mapper.Map<StockTradeHistoryGetModel>(d))
                .ToList();

            IEnumerable<StockSplitModel> splits = stockSplitService.Get(s => s.StockTickerId == trades[0].StockTickerId);

            if (splits.Any())
                ApplySplitsToTrades(trades, splits);

            return trades;
        }

        /// <inheritdoc/>
        public bool UserHasRightToStockTradeHistory(int stockTradeHistoryId, int userId)
            => repository.FindByCondition(a => a.Id == stockTradeHistoryId && a.UserIdentityId == userId).Count() == 1;

        /// <inheritdoc/>
        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker)
            => await stockDataInfluxRepo.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucket), new() { { "ticker", ticker } });

        /// <inheritdoc/>
        public async Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker, DateTime from)
            => await stockDataInfluxRepo.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucket), from, new() { { "ticker", ticker } });

        /// <inheritdoc/>
        public async Task<StockPrice> GetStockPriceAtDate(string ticker, DateTime atDate)
            => (await stockDataInfluxRepo.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucket), new Infl.DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "ticker", ticker } })).LastOrDefault();

        /// <inheritdoc/>
        public async Task<IEnumerable<StockPrice>> GetStocksPriceAtDate(string[] tickers, DateTime date)
        {
            List<Task<IEnumerable<StockPrice>>> finPriceTasks = new();

            for (int i = 0; i < tickers.Length; i++)
            {
                Task<IEnumerable<StockPrice>> taskTicker = stockDataInfluxRepo.GetAllData(new Infl.DataSourceIdentification(this.influxContext.OrganizationId, bucket), new Infl.DateTimeRange { From = date.AddDays(-5), To = date.AddDays(1) }, new() { { "ticker", tickers[i] } });
                finPriceTasks.Add(taskTicker);
            }

            IEnumerable<StockPrice>[] prices = await Task.WhenAll(finPriceTasks.ToArray());
            return prices.Where(m => m.Any()).Select(m => m.FirstOrDefault());
        }

        /// <summary>
        /// Applies stock splits to the trade history records.
        /// </summary>
        /// <param name="trades">The list of trade history records.</param>
        /// <param name="splits">The list of stock splits.</param>
        private void ApplySplitsToTrades(IEnumerable<StockTradeHistoryGetModel> trades, IEnumerable<StockSplitModel> splits)
        {
            foreach (StockTradeHistoryGetModel trade in trades)
            {
                double splitCoefficient = stockSplitService.GetAccumulatedCoefficient(splits.Where(c =>
                    c.SplitTimeStamp >= trade.TradeTimeStamp && c.StockTickerId == trade.StockTickerId));
                trade.TradeSizeAfterSplit = splitCoefficient * trade.TradeSize;
            }
        }

        /// <inheritdoc/>
        public void StoreReportToProcess(byte[] brokerFileData, int userId)
        {
            string fileContentBase64 = Convert.ToBase64String(brokerFileData);

            int stockTypeId = brokerReportTypeRepository.FindByCondition(t => t.Code == BrokerStockTypeCode).Single().Id;
            int stockStateId = brokerReportToProcessStateRepository.FindByCondition(t => t.Code == BrokerProcessStateCode).Single().Id;

            BrokerReportToProcess brokerReport = new BrokerReportToProcess
            {
                BrokerReportToProcessStateId = stockStateId,
                BrokerReportTypeId = stockTypeId,
                FileContentBase64 = fileContentBase64,
                ImportedTime = dateTimeProvider.Now.DateTimeInstance,
                UserIdentityId = userId
            };

            brokerReportToProcessRepository.Create(brokerReport);
            brokerReportToProcessRepository.Save();
        }
    }
}