using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData;
using BudgetManager.Repository;
using BudgetManager.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public class ComodityService : IComodityService
    {
        private const string bucketComodity = "Comodity";
        private const string organizationId = "f209a688c8dcfff3";
        private const string Gold = "AU";
        private readonly IComodityTradeHistoryRepository comodityTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IComodityTypeRepository comodityTypeRepository;
        private readonly IComodityUnitRepository comodityUnitRepository;
        private readonly InfluxDbData.IRepository<ComodityData> comodityRepository;

        public ComodityService(IComodityTradeHistoryRepository comodityTradeHistoryRepository, IUserIdentityRepository userIdentityRepository,
            IComodityTypeRepository comodityTypeRepository, IComodityUnitRepository comodityUnitRepository, InfluxDbData.IRepository<ComodityData> comodityRepository)
        {
            this.comodityTradeHistoryRepository = comodityTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.comodityTypeRepository = comodityTypeRepository;
            this.comodityUnitRepository = comodityUnitRepository;
            this.comodityRepository = comodityRepository;
        }

        public IEnumerable<ComodityTypeModel> GetComodityTypes()
        {

            return this.comodityTypeRepository.FindAll().Select(c => new ComodityTypeModel
            {
                Code = c.Code,
                Name = c.Name,
                ComodityUnitId = c.ComodityUnitId,
                Id = c.Id,
                ComodityUnit = c.ComodityUnit.Name
            });
        }

        public IEnumerable<ComodityUnitModel> GetComodityUnits()
        {
            return this.comodityUnitRepository.FindAll().Select(c => new ComodityUnitModel
            {
                Code = c.Code,
                Id = c.Id,
                Name = c.Name
            });
        }

        public IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin) =>
            this.GetComodityTradeHistoryForUser(this.userIdentityRepository.FindByCondition(u => u.Login == userLogin));

        public IEnumerable<ComodityTradeHistoryModel> GetByUser(int userId) =>
            this.GetComodityTradeHistoryForUser(this.userIdentityRepository.FindByCondition(u => u.Id == userId));

        private IEnumerable<ComodityTradeHistoryModel> GetComodityTradeHistoryForUser(IQueryable<UserIdentity> userIdentity) =>
            userIdentity.SelectMany(a => a.ComodityTradeHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.ComodityType)
                .ThenInclude(s => s.ComodityUnit)
                .Select(e => e.MapToComodityTradeHistoryModel());

        public void Update(ComodityTradeHistoryModel tradeHistory)
        {
            ComodityTradeHistory comodityTrade = this.userIdentityRepository.FindByCondition(u => u.Id == tradeHistory.UserIdentityId)
                .SelectMany(a => a.ComodityTradeHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.ComodityType)
                .ThenInclude(s => s.ComodityUnit)
                .SingleOrDefault(a => a.Id == tradeHistory.Id);

            if (comodityTrade is null)
                throw new Exception();

            comodityTrade.ComodityTypeId = tradeHistory.ComodityTypeId;
            comodityTrade.ComodityType = null;
            comodityTrade.CurrencySymbolId = tradeHistory.CurrencySymbolId;
            comodityTrade.CurrencySymbol = null;
            comodityTrade.TradeSize = tradeHistory.TradeSize;
            comodityTrade.TradeTimeStamp = tradeHistory.TradeTimeStamp;
            comodityTrade.TradeValue = tradeHistory.TradeValue;
            comodityTrade.Company = tradeHistory.Company;

            this.comodityTradeHistoryRepository.Update(comodityTrade);
            this.comodityTradeHistoryRepository.Save();
        }

        public bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId)
        {
            ComodityTradeHistory cryptoTrade = this.comodityTradeHistoryRepository.FindByCondition(a => a.Id == cryptoTradeId).Single();
            return cryptoTrade.UserIdentityId == userId;
        }

        public void Add(ComodityTradeHistoryModel tradeHistory)
        {
            this.comodityTradeHistoryRepository.Create(new ComodityTradeHistory()
            {
                TradeTimeStamp = tradeHistory.TradeTimeStamp,
                ComodityTypeId = tradeHistory.ComodityTypeId,
                CurrencySymbolId = tradeHistory.CurrencySymbolId,
                TradeSize = tradeHistory.TradeSize,
                TradeValue = tradeHistory.TradeValue,
                UserIdentityId = tradeHistory.UserIdentityId,
                Company = tradeHistory.Company
            });

            this.comodityTradeHistoryRepository.Save();
        }

        public void Delete(int id)
        {
            ComodityTradeHistory budget = this.comodityTradeHistoryRepository.FindByCondition(a => a.Id == id).Single();
            this.comodityTradeHistoryRepository.Delete(budget);
            this.comodityTradeHistoryRepository.Save();
        }

        public async Task<double> GetCurrentGoldPriceForOunce()
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketComodity);
            List<ComodityData> data = await this.comodityRepository.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false);
            return data.SingleOrDefault(a => string.Equals(a.Ticker, Gold))?.Price ?? 0;
        }
    }
}
