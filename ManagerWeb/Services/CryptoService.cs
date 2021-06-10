using BudgetManager.Data.DataModels;
using BudgetManager.InfluxDbData;
using BudgetManager.ManagerWeb.Extensions;
using BudgetManager.ManagerWeb.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.ManagerWeb.Services
{
    public class CryptoService : ICryptoService
    {
        private const string bucketForex = "Crypto";
        private const string organizationId = "8f46f33452affe4a";
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICryptoTradeHistoryRepository cryptoTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly InfluxDbData.IRepository<CryptoData> cryptoRepository;

        public CryptoService(IHttpContextAccessor httpContextAccessor, ICryptoTradeHistoryRepository cryptoTradeHistoryRepository, IUserIdentityRepository userIdentityRepository, InfluxDbData.IRepository<CryptoData> cryptoRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cryptoTradeHistoryRepository = cryptoTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.cryptoRepository = cryptoRepository;
        }

        public IEnumerable<TradeHistory> Get()
        {
            return this.userIdentityRepository.FindByCondition(u => u.Login == this.GetUserIdentity())
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Select(e => e.MapToOverViewViewModel());
        }

        public TradeHistory Get(int id)
        {
            return this.userIdentityRepository.FindByCondition(u => u.Login == this.GetUserIdentity())
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Where(s => s.Id == id)
                .Select(e => e.MapToOverViewViewModel())
                .Single();
        }

        public void Update(TradeHistory tradeHistory)
        {
            CryptoTradeHistory tradeHistoryRecord = this.userIdentityRepository.FindByCondition(u => u.Login == this.GetUserIdentity())
                .SelectMany(a => a.CryptoTradesHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.CryptoTicker)
                .Single(s => s.Id == tradeHistory.Id);

            if(tradeHistoryRecord == null)
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

        private string GetUserIdentity()
        {
            return this.httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
