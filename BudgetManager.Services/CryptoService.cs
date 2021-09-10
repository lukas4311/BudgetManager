using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BudgetManager.Services
{
    public class CryptoService : ICryptoService
    {
        private const string bucketForex = "Crypto";
        private const string organizationId = "8f46f33452affe4a";
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly InfluxDbData.IRepository<CryptoData> cryptoRepository;

        public CryptoService(ICryptoTradeHistoryRepository cryptoTradeHistoryRepository, IUserIdentityRepository userIdentityRepository, InfluxDbData.IRepository<CryptoData> cryptoRepository)
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

        public void Update(TradeHistory tradeHistory)
        {
            CryptoTradeHistory tradeHistoryRecord = this.userIdentityRepository.FindByCondition(u => u.Id == tradeHistory.Id)
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Single();

            if (tradeHistoryRecord is null)
                throw new Exception();

            tradeHistoryRecord.CryptoTickerId = tradeHistory.CryptoTickerId;
            tradeHistoryRecord.CurrencySymbolId = tradeHistory.CurrencySymbolId;
            tradeHistoryRecord.TradeSize = tradeHistory.TradeSize;
            tradeHistoryRecord.TradeTimeStamp = tradeHistory.TradeTimeStamp;
            tradeHistoryRecord.TradeValue = tradeHistory.TradeValue;

            this.cryptoTradeHistoryRepository.Update(tradeHistoryRecord);
        }

        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketForex);
            List<CryptoData> data = await this.cryptoRepository.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false);
            return data.SingleOrDefault(a => string.Equals(a.Ticker, $"{fromSymbol}{toSymbol}", System.StringComparison.OrdinalIgnoreCase))?.ClosePrice ?? 0;
        }
    }
}
