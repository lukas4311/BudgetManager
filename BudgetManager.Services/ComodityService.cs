using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class ComodityService : IComodityService
    {
        private readonly IComodityTradeHistoryRepository comodityTradeHistoryRepository;
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IComodityTypeRepository comodityTypeRepository;
        private readonly IComodityUnitRepository comodityUnitRepository;

        public ComodityService(IComodityTradeHistoryRepository comodityTradeHistoryRepository, IUserIdentityRepository userIdentityRepository,
            IComodityTypeRepository comodityTypeRepository, IComodityUnitRepository comodityUnitRepository)
        {
            this.comodityTradeHistoryRepository = comodityTradeHistoryRepository;
            this.userIdentityRepository = userIdentityRepository;
            this.comodityTypeRepository = comodityTypeRepository;
            this.comodityUnitRepository = comodityUnitRepository;
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
                UserIdentityId = tradeHistory.UserIdentityId
            });

            this.comodityTradeHistoryRepository.Save();
        }

        public void Delete(int id)
        {
            ComodityTradeHistory budget = this.comodityTradeHistoryRepository.FindByCondition(a => a.Id == id).Single();
            this.comodityTradeHistoryRepository.Delete(budget);
            this.comodityTradeHistoryRepository.Save();
        }
    }
}
