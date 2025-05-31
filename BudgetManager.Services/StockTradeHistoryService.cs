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
using System.Text.Json;
using System.Collections.Concurrent;

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
        private readonly IRepository<TickerAdjustedInfo> adjustedInfoRepository;

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
            IFinancialClient financialClient, IRepository<TickerAdjustedInfo> adjustedInfoRepository) : base(repository, mapper)
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
            this.adjustedInfoRepository = adjustedInfoRepository;
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
                double splitCoefficient = stockSplitService.GetAccumulatedCoefficient(splits.Where(c => c.SplitTimeStamp >= trade.TradeTimeStamp && c.StockTickerId == trade.StockTickerId));
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

        private Client.FinancialApiClient.CurrencySymbol? GetMetadataCurrency(EnumItem ticker)
        {
            if (ticker == null || string.IsNullOrEmpty(ticker.Metadata))
                return null;

            Dictionary<string, string> metadata;
            try
            {
                metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(ticker.Metadata);
            }
            catch (JsonException)
            {
                return null;
            }

            if (!metadata.TryGetValue("currency", out string tickerCurrency) || !Enum.TryParse(tickerCurrency, out Client.FinancialApiClient.CurrencySymbol tickerCurrencySymbol))
                return null;

            return tickerCurrencySymbol;
        }

        public async Task<IEnumerable<TradeGroupedTickerWithProfitLoss>> GetAllTradesGroupedByTickerWithProfitInfo(int userId, string currency)
        {
            ForexRateCache forexCache = new ForexRateCache((from, to) => this.financialClient.GetForexPairPriceAsync(from, to));
            IEnumerable<TradeGroupedTicker> data = this.GetAllTradesGroupedByTicker(userId);

            if (!Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol))
                throw new ArgumentException($"Invalid currency: {currency}");

            async Task<TradeGroupedTickerWithProfitLoss> ProcessTradeItem(TradeGroupedTicker item)
            {
                try
                {
                    if (!Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol))
                        return null;

                    EnumItem ticker = enumRepository.FindByCondition(e => e.Code == item.TickerCode).SingleOrDefault();
                    Client.FinancialApiClient.CurrencySymbol tickerCurrencySymbol = GetMetadataCurrency(ticker) ?? fromSymbol;

                    // Check for price_ticker in metadata  
                    string priceTicker = null;

                    // TODO: use new table to search price ticker

                    if (ticker != null && !string.IsNullOrEmpty(ticker.Metadata))
                    {
                        try
                        {
                            Dictionary<string, string> metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(ticker.Metadata);
                            metadata.TryGetValue("price_ticker", out priceTicker);
                        }
                        catch (JsonException)
                        {
                            // Continue processing  
                        }
                    }

                    // Use price_ticker if it exists, otherwise fallback to item.TickerCode  
                    string tickerToUse = priceTicker ?? item.TickerCode;

                    // Execute operations in parallel  
                    Task<StockPrice> stockPriceTask = GetStockPriceAtDate(tickerToUse, DateTime.Now);
                    Task<double> currencyPriceForTickerTask = forexCache.GetRateAsync(tickerCurrencySymbol, toSymbol);
                    Task<double> currencyPriceForValueTask = forexCache.GetRateAsync(fromSymbol, toSymbol);

                    await Task.WhenAll(stockPriceTask, currencyPriceForTickerTask, currencyPriceForValueTask);

                    StockPrice stockPrice = stockPriceTask.Result;
                    double currencyPriceForTicker = currencyPriceForTickerTask.Result;
                    double currencyPriceForValue = currencyPriceForValueTask.Result;

                    double convertedPrice = (stockPrice?.Price ?? 1) * currencyPriceForTicker;
                    double totalAccumulatedValue = item.AccumulatedSize * convertedPrice;
                    double totalPercentageProfitOrLoss = (Math.Abs(totalAccumulatedValue) / (Math.Abs(item.Value) * currencyPriceForValue)) * 100 - 100;

                    return new TradeGroupedTickerWithProfitLoss
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
                }
                catch (Exception)
                {
                    return null;
                }
            }

            await PreCacheForexRates(data, toSymbol, forexCache);
            IEnumerable<Task<TradeGroupedTickerWithProfitLoss>> tasks = data.Select(ProcessTradeItem);
            TradeGroupedTickerWithProfitLoss[] results = await Task.WhenAll(tasks);

            return results.Where(r => r != null);
        }

        public async Task<IEnumerable<TradesGroupedMonth>> GetAllTradesGroupedByMonthWithProfitInfo(int userId, string currency)
        {
            ForexRateCache forexCache = new ForexRateCache(
                (from, to) => this.financialClient.GetForexPairPriceAsync(from, to),
                (from, to, date) => this.financialClient.GetForexPairPriceAtDateAsync(from, to, date)
            );

            IEnumerable<TradesGroupedMonth> data = this.GetAllTradesGroupedByMonth(userId);

            if (!Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol))
                throw new ArgumentException($"Invalid currency: {currency}");

            async Task<TradesGroupedMonthWithProfitLoss> ProcessTradeItem(TradesGroupedMonth item)
            {
                try
                {
                    DateTime tradeDate = new DateTime(item.Year, item.Month, 1);

                    if (!Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol))
                        return null;

                    // Execute operations in parallel
                    Task<StockPrice> stockPriceTask = this.GetStockPriceAtDate(item.TickerCode, tradeDate);
                    Task<double> currencyPriceTask = forexCache.GetRateAtDateAsync(fromSymbol, toSymbol, tradeDate);

                    await Task.WhenAll(stockPriceTask, currencyPriceTask);

                    StockPrice stockPrice = stockPriceTask.Result;
                    double currencyPrice = currencyPriceTask.Result;

                    return new TradesGroupedMonthWithProfitLoss
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
                }
                catch (Exception)
                {
                    return null;
                }
            }

            // Pre-cache unique forex pairs for historical dates
            IEnumerable<(Client.FinancialApiClient.CurrencySymbol FromSymbol, Client.FinancialApiClient.CurrencySymbol ToSymbol, DateTime Date)> uniqueForexPairs = data
                .Where(item => Enum.TryParse<Client.FinancialApiClient.CurrencySymbol>(item.CurrencyCode, out _))
                .Select(item => (
                    FromSymbol: Enum.Parse<Client.FinancialApiClient.CurrencySymbol>(item.CurrencyCode),
                    ToSymbol: toSymbol,
                    Date: new DateTime(item.Year, item.Month, 1)
                ))
                .Distinct();

            IEnumerable<Task> cacheTasks = uniqueForexPairs.Select(async pair =>
            {
                try
                {
                    await forexCache.GetRateAtDateAsync(pair.FromSymbol, pair.ToSymbol, pair.Date);
                }
                catch (Exception)
                {
                    // Log error but continue
                }
            });

            await Task.WhenAll(cacheTasks);

            // Process all items in parallel
            IEnumerable<Task<TradesGroupedMonthWithProfitLoss>> tasks = data.Select(ProcessTradeItem);
            TradesGroupedMonthWithProfitLoss[] results = await Task.WhenAll(tasks);

            return results.Where(r => r != null);
        }

        public async Task<IEnumerable<TradeGroupedTradeTimeWithProfitLoss>> GetAllTradesGroupedByTradeDateWithProfitInfo(int userId, string currency)
        {
            ForexRateCache forexCache = new ForexRateCache(
                (from, to) => this.financialClient.GetForexPairPriceAsync(from, to),
                (from, to, date) => this.financialClient.GetForexPairPriceAtDateAsync(from, to, date)
            );

            IEnumerable<TradeGroupedTradeTime> data = this.GetAllTradesGroupedByTradeDate(userId);

            if (!Enum.TryParse(currency, out Client.FinancialApiClient.CurrencySymbol toSymbol))
                throw new ArgumentException($"Invalid currency: {currency}");

            async Task<TradeGroupedTradeTimeWithProfitLoss> ProcessTradeItem(TradeGroupedTradeTime item)
            {
                try
                {
                    if (!Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol))
                        return null;

                    // Execute operations in parallel
                    Task<StockPrice> stockPriceTask = this.GetStockPriceAtDate(item.TickerCode, item.TimeStamp);
                    Task<double> currencyPriceTask = forexCache.GetRateAtDateAsync(fromSymbol, toSymbol, item.TimeStamp);

                    await Task.WhenAll(stockPriceTask, currencyPriceTask);

                    StockPrice stockPrice = stockPriceTask.Result;
                    double currencyPrice = currencyPriceTask.Result;

                    return new TradeGroupedTradeTimeWithProfitLoss
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
                }
                catch (Exception)
                {
                    return null;
                }
            }

            // Pre-cache unique forex pairs for specific dates
            IEnumerable<(Client.FinancialApiClient.CurrencySymbol FromSymbol, Client.FinancialApiClient.CurrencySymbol ToSymbol, DateTime Date)> uniqueForexPairs = data
                .Where(item => Enum.TryParse<Client.FinancialApiClient.CurrencySymbol>(item.CurrencyCode, out _))
                .Select(item => (
                    FromSymbol: Enum.Parse<Client.FinancialApiClient.CurrencySymbol>(item.CurrencyCode),
                    ToSymbol: toSymbol,
                    Date: item.TimeStamp
                ))
                .Distinct();

            IEnumerable<Task> cacheTasks = uniqueForexPairs.Select(async pair =>
            {
                try
                {
                    await forexCache.GetRateAtDateAsync(pair.FromSymbol, pair.ToSymbol, pair.Date);
                }
                catch (Exception)
                {
                    // Log error but continue
                }
            });

            await Task.WhenAll(cacheTasks);

            // Process all items in parallel
            IEnumerable<Task<TradeGroupedTradeTimeWithProfitLoss>> tasks = data.Select(ProcessTradeItem);
            TradeGroupedTradeTimeWithProfitLoss[] results = await Task.WhenAll(tasks);

            return results.Where(r => r != null);
        }

        // Helper method for the ticker-based method
        private async Task PreCacheForexRates(IEnumerable<TradeGroupedTicker> data, Client.FinancialApiClient.CurrencySymbol toSymbol, ForexRateCache forexCache)
        {
            HashSet<(Client.FinancialApiClient.CurrencySymbol From, Client.FinancialApiClient.CurrencySymbol To)> uniqueForexPairs = new HashSet<(Client.FinancialApiClient.CurrencySymbol From, Client.FinancialApiClient.CurrencySymbol To)>();

            foreach (TradeGroupedTicker item in data)
            {
                if (Enum.TryParse(item.CurrencyCode, out Client.FinancialApiClient.CurrencySymbol fromSymbol))
                    uniqueForexPairs.Add((fromSymbol, toSymbol));

                EnumItem ticker = enumRepository.FindByCondition(e => e.Code == item.TickerCode).SingleOrDefault();

                if (ticker != null && !string.IsNullOrEmpty(ticker.Metadata))
                {
                    try
                    {
                        Dictionary<string, string> metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(ticker.Metadata);

                        if (metadata.TryGetValue("currency", out string tickerCurrency) && Enum.TryParse(tickerCurrency, out Client.FinancialApiClient.CurrencySymbol tickerCurrencySymbol))
                            uniqueForexPairs.Add((tickerCurrencySymbol, toSymbol));
                    }
                    catch (JsonException)
                    {
                        // Continue processing
                    }
                }
            }

            IEnumerable<Task> cacheTasks = uniqueForexPairs.Select(async pair =>
            {
                try
                {
                    await forexCache.GetRateAsync(pair.From, pair.To);
                }
                catch (Exception)
                {
                    // Log error but continue
                }
            });

            await Task.WhenAll(cacheTasks);
        }
    }

    public class ForexRateCache
    {
        private readonly ConcurrentDictionary<string, Lazy<Task<(double Rate, DateTime CachedAt)>>> _cache = new();
        private readonly TimeSpan _cacheExpiryTime;
        private readonly Func<Client.FinancialApiClient.CurrencySymbol, Client.FinancialApiClient.CurrencySymbol, Task<double>> _fetchRate;
        private readonly Func<Client.FinancialApiClient.CurrencySymbol, Client.FinancialApiClient.CurrencySymbol, DateTime, Task<double>> _fetchRateAtDate;

        public ForexRateCache(
            Func<Client.FinancialApiClient.CurrencySymbol, Client.FinancialApiClient.CurrencySymbol, Task<double>> fetchRate,
            Func<Client.FinancialApiClient.CurrencySymbol, Client.FinancialApiClient.CurrencySymbol, DateTime, Task<double>> fetchRateAtDate = null,
            TimeSpan? cacheExpiryTime = null)
        {
            _fetchRate = fetchRate;
            _fetchRateAtDate = fetchRateAtDate;
            _cacheExpiryTime = cacheExpiryTime ?? TimeSpan.FromMinutes(5);
        }

        public async Task<double> GetRateAsync(Client.FinancialApiClient.CurrencySymbol fromSymbol, Client.FinancialApiClient.CurrencySymbol toSymbol)
        {
            if (fromSymbol == toSymbol) return 1.0;

            string cacheKey = $"{fromSymbol}_{toSymbol}";
            return await GetOrFetchRate(cacheKey, () => _fetchRate(fromSymbol, toSymbol));
        }

        public async Task<double> GetRateAtDateAsync(Client.FinancialApiClient.CurrencySymbol fromSymbol, Client.FinancialApiClient.CurrencySymbol toSymbol, DateTime date)
        {
            if (fromSymbol == toSymbol) return 1.0;

            string cacheKey = $"{fromSymbol}_{toSymbol}_{date:yyyy-MM-dd}";
            return await GetOrFetchRate(cacheKey, () => _fetchRateAtDate(fromSymbol, toSymbol, date));
        }

        private async Task<double> GetOrFetchRate(string cacheKey, Func<Task<double>> fetchFunc)
        {
            Lazy<Task<(double Rate, DateTime CachedAt)>> lazyTask = _cache.GetOrAdd(cacheKey, _ => new Lazy<Task<(double Rate, DateTime CachedAt)>>(async () =>
            {
                double rate = await fetchFunc();
                return (rate == 0 ? 1 : rate, DateTime.UtcNow);
            }));

            (double rate, DateTime cachedAt) = await lazyTask.Value;

            if (DateTime.UtcNow - cachedAt >= _cacheExpiryTime)
            {
                _cache.TryRemove(cacheKey, out _);
                return await GetOrFetchRate(cacheKey, fetchFunc);
            }

            return rate;
        }
    }
}