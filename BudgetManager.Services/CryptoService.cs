using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using Infl = BudgetManager.InfluxDbData;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using BudgetManager.Domain.Enums;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Services.SqlQuery;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class CryptoService : BaseService<TradeHistory, Trade, IRepository<Trade>>, ICryptoService
    {
        private const string bucketCrypto = "Crypto";
        private const string bucketCryptoV2 = "CryptoV2";
        private const string BrokerCryptoTypeCode = "Crypto";
        private const string BrokerProcessStateCode = "InProcess";
        private readonly IRepository<Trade> cryptoTradeHistoryRepository;
        private readonly IRepository<UserIdentity> userIdentityRepository;
        private readonly Infl.IInfluxContext influxContext;
        private readonly Infl.IRepository<Infl.CryptoData> cryptoRepository;
        private readonly Infl.IRepository<Infl.CryptoDataV2> cryptoRepositoryV2;
        private readonly IMapper autoMapper;
        private readonly IRepository<BrokerReportType> brokerReportTypeRepository;
        private readonly IRepository<BrokerReportToProcessState> brokerReportToProcessStateRepository;
        private readonly IRepository<BrokerReportToProcess> brokerReportToProcessRepository;
        private readonly IRepository<Trade> tradeRepository;
        private readonly IRepository<EnumItem> enumRepository;
        private readonly IDateTime dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoService"/> class.
        /// </summary>
        /// <param name="cryptoTradeHistoryRepository">The cryptocurrency trade history repository.</param>
        /// <param name="userIdentityRepository">The user identity repository.</param>
        /// <param name="influxContext">The InfluxDB context.</param>
        /// <param name="cryptoRepository">The cryptocurrency repository.</param>
        /// <param name="cryptoRepositoryV2">The cryptocurrency repository for version 2.</param>
        /// <param name="autoMapper">The AutoMapper instance.</param>
        /// <param name="brokerReportTypeRepository">The broker report type repository.</param>
        /// <param name="brokerReportToProcessStateRepository">The broker report to process state repository.</param>
        /// <param name="brokerReportToProcessRepository">The broker report to process repository.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        public CryptoService(IRepository<Trade> cryptoTradeHistoryRepository, IRepository<UserIdentity> userIdentityRepository, Infl.IInfluxContext influxContext,
            Infl.IRepository<Infl.CryptoData> cryptoRepository, Infl.IRepository<Infl.CryptoDataV2> cryptoRepositoryV2, IMapper autoMapper,
            IRepository<BrokerReportType> brokerReportTypeRepository, IRepository<BrokerReportToProcessState> brokerReportToProcessStateRepository,
            IRepository<BrokerReportToProcess> brokerReportToProcessRepository, IRepository<Trade> tradeRepository, IRepository<EnumItem> enumRepository, IDateTime dateTimeProvider) : base(cryptoTradeHistoryRepository, autoMapper)
        {
            this.cryptoTradeHistoryRepository = cryptoTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.influxContext = influxContext;
            this.cryptoRepository = cryptoRepository;
            this.cryptoRepositoryV2 = cryptoRepositoryV2;
            this.autoMapper = autoMapper;
            this.brokerReportTypeRepository = brokerReportTypeRepository;
            this.brokerReportToProcessStateRepository = brokerReportToProcessStateRepository;
            this.brokerReportToProcessRepository = brokerReportToProcessRepository;
            this.tradeRepository = tradeRepository;
            this.enumRepository = enumRepository;
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public IEnumerable<TradeHistory> GetByUser(string userLogin)
        {
            return tradeRepository.FindAll()
                .Include(t => t.UserIdentity)
                .Include(t => t.TradeCurrencySymbol)
                .Include(t => t.Ticker)
                .ThenInclude(t => t.EnumItemType)
                .Where(u => u.UserIdentity.Login == userLogin && u.Ticker.EnumItemType.Code == nameof(EEnumTypes.CryptoTradeTickers))
                .Select(t => mapper.Map<TradeHistory>(t));
        }

        /// <inheritdoc/>
        public IEnumerable<TradeHistory> GetByUser(int userId)
        {
            return tradeRepository.FindAll()
                .Include(t => t.UserIdentity)
                .Include(t => t.TradeCurrencySymbol)
                .Include(t => t.Ticker)
                .ThenInclude(t => t.EnumItemType)
                .Where(u => u.UserIdentityId == userId && u.Ticker.EnumItemType.Code == nameof(EEnumTypes.CryptoTradeTickers))
                .Select(t => mapper.Map<TradeHistory>(t));
        }

        /// <inheritdoc/>
        public TradeHistory Get(int id, int userId)
        {
            return tradeRepository.FindAll()
                .Include(t => t.UserIdentity)
                .Include(t => t.TradeCurrencySymbol)
                .Include(t => t.Ticker)
                .ThenInclude(t => t.EnumItemType)
                .Where(u => u.Id == id && u.Ticker.EnumItemType.Code == nameof(EEnumTypes.CryptoTradeTickers))
                .Select(t => mapper.Map<TradeHistory>(t))
                .Single();
        }

        /// <inheritdoc/>
        public bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId)
        {
            Trade cryptoTrade = cryptoTradeHistoryRepository.FindByCondition(a => a.Id == cryptoTradeId).Single();
            return cryptoTrade.UserIdentityId == userId;
        }

        /// <inheritdoc/>
        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol)
        {
            Infl.DataSourceIdentification dataSourceIdentification = new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketCrypto);
            IEnumerable<Infl.CryptoData> data = await cryptoRepository.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false);
            return data.SingleOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }

        /// <inheritdoc/>
        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol, DateTime atDate)
        {
            Infl.DataSourceIdentification dataSourceIdentification = new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketCrypto);
            IEnumerable<Infl.CryptoData> data = await cryptoRepository.GetAllData(dataSourceIdentification,
                new Infl.DateTimeRange { From = atDate, To = atDate.AddDays(1) }, new() { { "ticker", $"{fromSymbol}{toSymbol}" } }).ConfigureAwait(false);
            return data.FirstOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Infl.CryptoDataV2>> GetCryptoPriceHistory(string ticker)
            => await cryptoRepositoryV2.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketCryptoV2), new() { { "ticker", ticker } });

        /// <inheritdoc/>
        public async Task<IEnumerable<Infl.CryptoDataV2>> GetCryptoPriceHistory(string ticker, DateTime from)
            => await cryptoRepositoryV2.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketCryptoV2), from, new() { { "ticker", ticker } });

        /// <inheritdoc/>
        public async Task<Infl.CryptoDataV2> GetCryptoPriceAtDate(string ticker, DateTime atDate)
            => (await cryptoRepositoryV2.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketCryptoV2), new Infl.DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "ticker", ticker } })).LastOrDefault();

        /// <inheritdoc/>
        public void StoreReportToProcess(byte[] brokerFileData, int userId, int brokerId)
        {
            string fileContentBase64 = Convert.ToBase64String(brokerFileData);

            int stockTypeId = brokerReportTypeRepository.FindByCondition(t => t.Code == BrokerCryptoTypeCode).Single().Id;
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

        public IEnumerable<CryptoTicker> GetAllTickers()
        {
            return enumRepository.FindAll().Include(t => t.EnumItemType).Where(t => t.EnumItemType.Code == nameof(EEnumTypes.CryptoTradeTickers)).Select(t => mapper.Map<CryptoTicker>(t));
        }

        public IEnumerable<TradesGroupedMonth> GetAllTradesGroupedByMonth(int userId)
            => brokerReportToProcessRepository.FromSql<TradesGroupedMonth>(StockTradeQueries.GetAllTradesWithSplitGroupedByMonthAndTicker__TradeTable(userId, TickerTypes.CryptoTradeTickers));

        public IEnumerable<TradeGroupedTicker> GetAllTradesGroupedByTicker(int userId)
            => brokerReportToProcessRepository.FromSql<TradeGroupedTicker>(StockTradeQueries.GetAllTradesGroupedByTicker__TradeTable(userId, TickerTypes.CryptoTradeTickers));

        public IEnumerable<TradeGroupedTradeTime> GetAllTradesGroupedByTradeDate(int userId)
            => brokerReportToProcessRepository.FromSql<TradeGroupedTradeTime>(StockTradeQueries.GetAllTradesGroupedByTickerAndTradeDate__TradeTable(userId, TickerTypes.CryptoTradeTickers));
    }
}
