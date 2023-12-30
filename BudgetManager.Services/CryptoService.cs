using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    public class CryptoService : BaseService<TradeHistory, CryptoTradeHistory, ICryptoTradeHistoryRepository>, ICryptoService
    {
        private const string bucketCrypto = "Crypto";
        private const string bucketCryptoV2 = "CryptoV2";
        private const string BrokerStockTypeCode = "Stock";
        private const string BrokerProcessStateCode = "InProcess";
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IInfluxContext influxContext;
        private readonly InfluxDbData.IRepository<CryptoData> cryptoRepository;
        private readonly InfluxDbData.IRepository<CryptoDataV2> cryptoRepositoryV2;
        private readonly IMapper autoMapper;
        private readonly IBrokerReportTypeRepository brokerReportTypeRepository;
        private readonly IBrokerReportToProcessStateRepository brokerReportToProcessStateRepository;
        private readonly IBrokerReportToProcessRepository brokerReportToProcessRepository;
        private readonly IDateTime dateTimeProvider;

        public CryptoService(ICryptoTradeHistoryRepository cryptoTradeHistoryRepository, IUserIdentityRepository userIdentityRepository, IInfluxContext influxContext,
            InfluxDbData.IRepository<CryptoData> cryptoRepository, InfluxDbData.IRepository<CryptoDataV2> cryptoRepositoryV2, IMapper autoMapper,
            IBrokerReportTypeRepository brokerReportTypeRepository, IBrokerReportToProcessStateRepository brokerReportToProcessStateRepository,
            IBrokerReportToProcessRepository brokerReportToProcessRepository, IDateTime dateTimeProvider) : base(cryptoTradeHistoryRepository, autoMapper)
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
            this.dateTimeProvider = dateTimeProvider;
        }

        public IEnumerable<TradeHistory> GetByUser(string userLogin)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Login == userLogin)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Select(e => e.MapToOverViewViewModel());
        }

        public IEnumerable<TradeHistory> GetByUser(int userId)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Id == userId)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Select(e => e.MapToOverViewViewModel());
        }

        public TradeHistory Get(int id, int userId)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Id == userId)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Where(s => s.Id == id)
                .Select(e => e.MapToOverViewViewModel())
                .Single();
        }

        public bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId)
        {
            CryptoTradeHistory cryptoTrade = this.cryptoTradeHistoryRepository.FindByCondition(a => a.Id == cryptoTradeId).Single();
            return cryptoTrade.UserIdentityId == userId;
        }

        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(this.influxContext.OrganizationId, bucketCrypto);
            IEnumerable<CryptoData> data = await this.cryptoRepository.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false);
            return data.SingleOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }

        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol, DateTime atDate)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(this.influxContext.OrganizationId, bucketCrypto);
            IEnumerable<CryptoData> data = await this.cryptoRepository.GetAllData(dataSourceIdentification,
                new DateTimeRange { From = atDate, To = atDate.AddDays(1) }, new() { { "ticker", $"{fromSymbol}{toSymbol}" } }).ConfigureAwait(false);
            return data.FirstOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }

        public async Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker)
            => await cryptoRepositoryV2.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucketCryptoV2), new() { { "ticker", ticker } });

        public async Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker, DateTime from)
            => await cryptoRepositoryV2.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucketCryptoV2), from, new() { { "ticker", ticker } });

        public async Task<CryptoDataV2> GetCryptoPriceAtDate(string ticker, DateTime atDate)
            => (await cryptoRepositoryV2.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucketCryptoV2), new DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "ticker", ticker } })).LastOrDefault();

        public void StoreReportToProcess(byte[] brokerFileData, int userId)
        {
            string fileContentBase64 = Convert.ToBase64String(brokerFileData);

            var stockTypeId = this.brokerReportTypeRepository.FindByCondition(t => t.Code == BrokerStockTypeCode).Single().Id;
            var stockStateId = this.brokerReportToProcessStateRepository.FindByCondition(t => t.Code == BrokerProcessStateCode).Single().Id;

            var brokerReport = new BrokerReportToProcess
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
