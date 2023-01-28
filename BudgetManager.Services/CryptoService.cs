using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private const string bucketForex = "Crypto";
        private const string organizationId = "f209a688c8dcfff3";
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly InfluxDbData.IRepository<CryptoData> cryptoRepository;

        public CryptoService(ICryptoTradeHistoryRepository cryptoTradeHistoryRepository, IUserIdentityRepository userIdentityRepository, 
            InfluxDbData.IRepository<CryptoData> cryptoRepository, IMapper autoMapper) : base(cryptoTradeHistoryRepository, autoMapper)
        {
            this.cryptoTradeHistoryRepository = cryptoTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.cryptoRepository = cryptoRepository;
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
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketForex);
            IEnumerable<CryptoData> data = await this.cryptoRepository.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false);
            return data.SingleOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }
    }
}
