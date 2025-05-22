using AutoMapper;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Domain.Enums;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.SqlQuery;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infl = BudgetManager.InfluxDbData;
using IFinancialClient = BudgetManager.Client.FinancialApiClient.IFinancialClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class StockTradeHistoryService : BaseService<StockTradeHistoryModel, Trade, IRepository<Trade>>, IStockTradeHistoryService
    {
        private const string bucket = "StockPrice";
        private const string BrokerStockTypeCode = "Stock";
        private const string BrokerProcessStateCode = "InProcess";
        private readonly IRepository<Trade> repository;
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
        private readonly IRepository<EnumItem> enumRepository;
        private readonly IFinancialClient financialClient;

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
        public StockTradeHistoryService(IRepository<Trade> repository, IMapper mapper,
            InfluxDbData.IRepository<StockPrice> stockDataInfluxRepo, Infl.IInfluxContext influxContext,
            IStockSplitService stockSplitService, IForexService forexService, IRepository<CurrencySymbol> currencySymbolRepository,
            IRepository<BrokerReportType> brokerReportTypeRepository, IRepository<BrokerReportToProcessState> brokerReportToProcessStateRepository,
            IRepository<BrokerReportToProcess> brokerReportToProcessRepository, IDateTime dateTimeProvider, IRepository<EnumItem> enumRepository,
            IFinancialClient financialClient) : base(repository, mapper)
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
            this.enumRepository = enumRepository;
            this.financialClient = financialClient;
        }


        /// <inheritdoc/>
        public IEnumerable<StockTradeHistoryGetModel> GetAll(int userId)
        {
            List<StockTradeHistoryGetModel> trades = repository
                .FindByCondition(i => i.UserIdentityId == userId)
                .Include(t => t.TradeCurrencySymbol)
                .Include(t => t.Ticker)
                .ThenInclude(t => t.EnumItemType)
                .Where(t => t.Ticker.EnumItemType.Code == nameof(EEnumTypes.StockTradeTickers))
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
                .Include(t => t.TradeCurrencySymbol)
                .Include(t => t.Ticker)
                .ThenInclude(t => t.EnumItemType)
                .Where(t => t.Ticker.EnumItemType.Code == nameof(EEnumTypes.StockTradeTickers))
                .Select(t => mapper.Map<StockTradeHistoryGetModel>(t))
                .ToList();

            IEnumerable<StockSplitModel> splits = stockSplitService.GetAll();

            if (splits.Any())
                ApplySplitsToTrades(trades, splits);

            CurrencySymbol currencySymbolEntity = currencySymbolRepository.FindByCondition(a => a.Symbol == currencySymbol.ToString()).Single();
            bool notReachableData = false;

            for (int i = 0; i < trades.Count; i++)
            {
                StockTradeHistoryGetModel trade = trades[i];
                double exchangeRate = 0;

                if (!notReachableData)
                {
                    try
                    {
                        exchangeRate = await forexService.GetExchangeRate(trade.CurrencySymbol, currencySymbol.ToString(), trade.TradeTimeStamp);
                    }
                    catch (WebException)
                    {
                        notReachableData = true;
                    }
                    catch (Exception) { }
                }

                trade.TradeValue *= exchangeRate;
                trade.CurrencySymbol = currencySymbol.ToString();
                trade.CurrencySymbolId = currencySymbolEntity.Id;
            }

            return trades;
        }

        /// <inheritdoc/>
        public IEnumerable<StockTradeHistoryGetModel> GetTradeHistory(int userId, string stockTicker)
        {
            List<StockTradeHistoryGetModel> trades = repository.FindAll()
                .Include(t => t.Ticker)
                .Include(t => t.TradeCurrencySymbol)
                .Where(t => t.Ticker.Code == stockTicker && t.UserIdentityId == userId)
                .Select(t => mapper.Map<StockTradeHistoryGetModel>(t))
                .ToList();

            IEnumerable<StockSplitModel> splits = stockSplitService.Get(s => s.TickerId == trades[0].StockTickerId);

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
                Task<IEnumerable<StockPrice>> taskTicker = stockDataInfluxRepo.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucket), new Infl.DateTimeRange { From = date.AddDays(-5), To = date.AddDays(1) }, new() { { "ticker", tickers[i] } });
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
        public void StoreReportToProcess(byte[] brokerFileData, int userId, int brokerId)
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
                UserIdentityId = userId,
                BrokerId = brokerId
            };

            brokerReportToProcessRepository.Create(brokerReport);
            brokerReportToProcessRepository.Save();
        }

        public IEnumerable<TradesGroupedMonth> GetAllTradesGroupedByMonth(int userId)
            => brokerReportToProcessRepository.FromSqlRaw<TradesGroupedMonth>(StockTradeQueries.GetAllTradesWithSplitGroupedByMonthAndTicker__TradeTable(), userId, nameof(TickerTypes.StockTradeTickers));

        public IEnumerable<TradeGroupedTicker> GetAllTradesGroupedByTicker(int userId)
            => brokerReportToProcessRepository.FromSqlRaw<TradeGroupedTicker>(StockTradeQueries.GetAllTradesGroupedByTicker__TradeTable(), userId, nameof(TickerTypes.StockTradeTickers));

        public IEnumerable<TradeGroupedTradeTime> GetAllTradesGroupedByTradeDate(int userId)
            => brokerReportToProcessRepository.FromSqlRaw<TradeGroupedTradeTime>(StockTradeQueries.GetAllTradesGroupedByTickerAndTradeDate__TradeTable(), userId, nameof(TickerTypes.StockTradeTickers));

        public async Task<IEnumerable<TradesGroupedMonth>> GetAllTradesGroupedByMonthWithProfitInfo(int userId, string currency)
        {
            IEnumerable<TradesGroupedMonth> data = this.GetAllTradesGroupedByMonth(userId);
            List<TradesGroupedMonthWithProfitLoss> dataWithProfit = new List<TradesGroupedMonthWithProfitLoss>();
            Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol);

            foreach (TradesGroupedMonth item in data)
            {
                var tradeDate = new DateTime(item.Year, item.Month, 1);
                Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol);
                StockPrice stockPrice = await this.GetStockPriceAtDate(item.TickerCode, tradeDate);
                double currencyPrice = await this.financialClient.GetForexPairPriceAtDateAsync(fromSymbol, toSymbol, tradeDate);
                currencyPrice = currencyPrice == 0 ? 1 : currencyPrice;
                TradesGroupedMonthWithProfitLoss profitData = new TradesGroupedMonthWithProfitLoss
                {
                    TickerId = item.TickerId,
                    Year = item.Year,
                    Month = item.Month,
                    Size = item.Size,
                    Value = item.Value,
                    AccumulatedSize = item.AccumulatedSize,
                    CurrencySymbolId = item.CurrencySymbolId,
                    TickerCode = item.TickerCode,
                    CurrencyCode = currency,
                    TotalAccumulatedValue = item.AccumulatedSize * (stockPrice?.Price ?? 1) * currencyPrice,
                    TotalPercentageProfitOrLoss = (item.AccumulatedSize * (stockPrice?.Price ?? 1) * currencyPrice) / (item.Value * currencyPrice) * 100
                };
                dataWithProfit.Add(profitData);
            }

            return dataWithProfit;
        }

        public async Task<IEnumerable<TradeGroupedTickerWithProfitLoss>> GetAllTradesGroupedByTickerWithProfitInfo(int userId, string currency)
        {
            IEnumerable<TradeGroupedTicker> data = this.GetAllTradesGroupedByTicker(userId);
            List<TradeGroupedTickerWithProfitLoss> dataWithProfit = new List<TradeGroupedTickerWithProfitLoss>();
            Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol);

            foreach (TradeGroupedTicker item in data)
            {
                Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol);

                var ticker = enumRepository.FindByCondition(e => e.Code == item.TickerCode).SingleOrDefault();

                if (ticker == null || string.IsNullOrEmpty(ticker.Metadata))
                    throw new InvalidOperationException("Ticker or metadata not found.");

                var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(ticker.Metadata);

                if (!metadata.TryGetValue("currency", out var tickerCurrency))
                    continue;

                Enum.TryParse(tickerCurrency, out Client.FinancialApiClient.CurrencySymbol tickerCurrencySymbol);

                // Get stock price and convert  
                StockPrice stockPrice = await this.GetStockPriceAtDate(item.TickerCode, DateTime.Now);
                double currencyPriceForTicker = await this.financialClient.GetForexPairPriceAsync(tickerCurrencySymbol, toSymbol);
                double currencyPriceForValue = await this.financialClient.GetForexPairPriceAsync(fromSymbol, toSymbol);
                double convertedPrice = (stockPrice?.Price ?? 1) * currencyPriceForTicker;
                currencyPriceForTicker = currencyPriceForTicker == 0 ? 1 : currencyPriceForTicker;
                currencyPriceForValue = currencyPriceForValue == 0 ? 1 : currencyPriceForValue;

                double totalAccumulatedValue = item.AccumulatedSize * convertedPrice;
                double totalPercentageProfitOrLoss = (Math.Abs(totalAccumulatedValue) / (Math.Abs(item.Value) * currencyPriceForValue)) * 100 - 100;
                //double totalPercentageProfitOrLoss = ((totalAccumulatedValue - (item.Value * currencyPriceForValue)) / (item.Value * currencyPriceForValue)) * 100;

                TradeGroupedTickerWithProfitLoss profitData = new TradeGroupedTickerWithProfitLoss
                {
                    TickerId = item.TickerId,
                    Size = item.Size,
                    Value = item.Value * currencyPriceForValue,
                    AccumulatedSize = item.AccumulatedSize,
                    CurrencySymbolId = item.CurrencySymbolId,
                    TickerCode = item.TickerCode,
                    CurrencyCode = currency,
                    TotalAccumulatedValue = totalAccumulatedValue,
                    TotalPercentageProfitOrLoss = totalPercentageProfitOrLoss
                };
                dataWithProfit.Add(profitData);
            }

            return dataWithProfit;
        }

        public async Task<IEnumerable<TradeGroupedTradeTimeWithProfitLoss>> GetAllTradesGroupedByTradeDateWithProfitInfo(int userId, string currency)
        {
            IEnumerable<TradeGroupedTradeTime> data = this.GetAllTradesGroupedByTradeDate(userId);
            List<TradeGroupedTradeTimeWithProfitLoss> dataWithProfit = new List<TradeGroupedTradeTimeWithProfitLoss>();
            Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol);

            foreach (TradeGroupedTradeTime item in data)
            {
                Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol);
                StockPrice stockPrice = await this.GetStockPriceAtDate(item.TickerCode, item.TimeStamp);
                double currencyPrice = await this.financialClient.GetForexPairPriceAtDateAsync(fromSymbol, toSymbol, item.TimeStamp);
                currencyPrice = currencyPrice == 0 ? 1 : currencyPrice;
                TradeGroupedTradeTimeWithProfitLoss profitData = new TradeGroupedTradeTimeWithProfitLoss
                {
                    TickerId = item.TickerId,
                    TimeStamp = item.TimeStamp,
                    Size = item.Size,
                    Value = item.Value,
                    AccumulatedSize = item.AccumulatedSize,
                    CurrencySymbolId = item.CurrencySymbolId,
                    TickerCode = item.TickerCode,
                    CurrencyCode = currency,
                    TotalAccumulatedValue = item.AccumulatedSize * (stockPrice?.Price ?? 1) * currencyPrice,
                    TotalPercentageProfitOrLoss = (item.AccumulatedSize * (stockPrice?.Price ?? 1) * currencyPrice) / (item.Value * currencyPrice) * 100
                };
                dataWithProfit.Add(profitData);
            }

            return dataWithProfit;
        }
    }
}