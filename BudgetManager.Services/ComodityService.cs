using AutoMapper;
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
    public class ComodityService : BaseService<ComodityTradeHistoryModel, ComodityTradeHistory, IComodityTradeHistoryRepository>, IComodityService
    {
        private const string bucketComodityV2 = "ComodityV2";
        private const string GoldTicker = "Gold";
        private readonly IComodityTradeHistoryRepository comodityTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IComodityTypeRepository comodityTypeRepository;
        private readonly IComodityUnitRepository comodityUnitRepository;
        private readonly IInfluxContext influxContext;
        private readonly InfluxDbData.IRepository<ComodityData> comodityRepository;
        private readonly InfluxDbData.IRepository<ComodityDataV2> comodityRepositoryV2;

        public ComodityService(IComodityTradeHistoryRepository comodityTradeHistoryRepository, IUserIdentityRepository userIdentityRepository,
            IComodityTypeRepository comodityTypeRepository, IComodityUnitRepository comodityUnitRepository, IInfluxContext influxContext,
            InfluxDbData.IRepository<ComodityData> comodityRepository, IMapper autoMapper, InfluxDbData.IRepository<ComodityDataV2> comodityRepositoryV2) : base(comodityTradeHistoryRepository, autoMapper)
        {
            this.comodityTradeHistoryRepository = comodityTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.comodityTypeRepository = comodityTypeRepository;
            this.comodityUnitRepository = comodityUnitRepository;
            this.influxContext = influxContext;
            this.comodityRepository = comodityRepository;
            this.comodityRepositoryV2 = comodityRepositoryV2;
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

        public bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId)
        {
            ComodityTradeHistory cryptoTrade = this.comodityTradeHistoryRepository.FindByCondition(a => a.Id == cryptoTradeId).Single();
            return cryptoTrade.UserIdentityId == userId;
        }

        public async Task<double> GetCurrentGoldPriceForOunce()
        {
            ComodityDataV2 data2 = (await comodityRepositoryV2.GetAllData(new DataSourceIdentification(this.influxContext.OrganizationId, bucketComodityV2), new DateTimeRange { From = DateTime.Now.AddDays(-5), To = DateTime.Now.AddDays(1) },
                new() { { "ticker", GoldTicker } })).LastOrDefault();
            return data2?.Price ?? 0;
        }
    }
}
