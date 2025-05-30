﻿using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using Infl = BudgetManager.InfluxDbData;
using BudgetManager.Repository;
using BudgetManager.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Services.SqlQuery;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class ComodityService : BaseService<ComodityTradeHistoryModel, ComodityTradeHistory, IRepository<ComodityTradeHistory>>, IComodityService
    {
        private const string bucketComodityV2 = "ComodityV2";
        private const string GoldTicker = "Gold";
        private readonly IRepository<ComodityTradeHistory> comodityTradeHistoryRepository;
        private readonly IRepository<UserIdentity> userIdentityRepository;
        private readonly IRepository<ComodityType> comodityTypeRepository;
        private readonly IRepository<ComodityUnit> comodityUnitRepository;
        private readonly Infl.IInfluxContext influxContext;
        private readonly Infl.IRepository<Infl.ComodityDataV2> comodityRepositoryV2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComodityService"/> class.
        /// </summary>
        /// <param name="comodityTradeHistoryRepository">The commodity trade history repository.</param>
        /// <param name="userIdentityRepository">The user identity repository.</param>
        /// <param name="comodityTypeRepository">The commodity type repository.</param>
        /// <param name="comodityUnitRepository">The commodity unit repository.</param>
        /// <param name="influxContext">The InfluxDB context.</param>
        /// <param name="comodityRepository">The commodity repository.</param>
        /// <param name="autoMapper">The AutoMapper instance.</param>
        /// <param name="comodityRepositoryV2">The commodity repository for version 2.</param>
        public ComodityService(IRepository<ComodityTradeHistory> comodityTradeHistoryRepository, IRepository<UserIdentity> userIdentityRepository,
            IRepository<ComodityType> comodityTypeRepository, IRepository<ComodityUnit> comodityUnitRepository, Infl.IInfluxContext influxContext,
            Infl.IRepository<Infl.ComodityData> comodityRepository, IMapper autoMapper, Infl.IRepository<Infl.ComodityDataV2> comodityRepositoryV2) : base(comodityTradeHistoryRepository, autoMapper)
        {
            this.comodityTradeHistoryRepository = comodityTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.comodityTypeRepository = comodityTypeRepository;
            this.comodityUnitRepository = comodityUnitRepository;
            this.influxContext = influxContext;
            this.comodityRepositoryV2 = comodityRepositoryV2;
        }

        /// <inheritdoc/>
        public IEnumerable<ComodityTypeModel> GetComodityTypes()
        {
            return comodityTypeRepository.FindAll().Select(c => new ComodityTypeModel
            {
                Code = c.Code,
                Name = c.Name,
                ComodityUnitId = c.ComodityUnitId,
                Id = c.Id,
                ComodityUnit = c.ComodityUnit.Name
            });
        }

        /// <inheritdoc/>
        public IEnumerable<ComodityUnitModel> GetComodityUnits()
        {
            return comodityUnitRepository.FindAll().Select(c => new ComodityUnitModel
            {
                Code = c.Code,
                Id = c.Id,
                Name = c.Name
            });
        }

        /// <inheritdoc/>
        public IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin) =>
            GetComodityTradeHistoryForUser(userIdentityRepository.FindByCondition(u => u.Login == userLogin));

        /// <inheritdoc/>
        public IEnumerable<ComodityTradeHistoryModel> GetByUser(int userId) =>
            GetComodityTradeHistoryForUser(userIdentityRepository.FindByCondition(u => u.Id == userId));

        /// <inheritdoc/>
        private IEnumerable<ComodityTradeHistoryModel> GetComodityTradeHistoryForUser(IQueryable<UserIdentity> userIdentity) =>
            userIdentity.SelectMany(a => a.ComodityTradeHistory)
                .Include(s => s.CurrencySymbol)
                .Include(s => s.ComodityType)
                .ThenInclude(s => s.ComodityUnit)
                .Select(e => e.MapToComodityTradeHistoryModel());

        /// <inheritdoc/>
        public bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId)
        {
            ComodityTradeHistory cryptoTrade = comodityTradeHistoryRepository.FindByCondition(a => a.Id == cryptoTradeId).Single();
            return cryptoTrade.UserIdentityId == userId;
        }

        /// <inheritdoc/>
        public async Task<double> GetCurrentGoldPriceForOunce()
        {
            Infl.ComodityDataV2 goldPriceHisotry = (await comodityRepositoryV2.GetAllData(new Infl.DataSourceIdentification(influxContext.OrganizationId, bucketComodityV2), new Infl.DateTimeRange { From = DateTime.Now.AddDays(-5), To = DateTime.Now.AddDays(1) },
                new() { { "ticker", GoldTicker } })).LastOrDefault();
            return goldPriceHisotry?.Price ?? 0;
        }

        /// <inheritdoc/>
        public IEnumerable<ComodityTradesGroupedMonth> GetAllTradesGroupedByMonth(int userId)
            => comodityUnitRepository.FromSqlRaw<ComodityTradesGroupedMonth>(ComodityQueries.GetAllComodityTradeSizeAndValue(), userId);

        /// <inheritdoc/>
        public IEnumerable<ComodityTradeGroupedTicker> GetAllTradesGroupedByTicker(int userId)
            => comodityUnitRepository.FromSqlRaw<ComodityTradeGroupedTicker>(ComodityQueries.GetAllComodityAccumulatedSizeAndValueInMonths(), userId);
    }

}
