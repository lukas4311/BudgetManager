﻿using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class OtherInvestmentService : BaseService<OtherInvestmentModel, OtherInvestment, IRepository<OtherInvestment>>, IOtherInvestmentService
    {
        private readonly IRepository<OtherInvestmentBalaceHistory> otherInvestmentBalaceHistoryRepository;
        private readonly IOtherInvestmentTagService otherInvestmentTagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherInvestmentService"/> class.
        /// </summary>
        /// <param name="repository">The other investment repository.</param>
        /// <param name="otherInvestmentBalaceHistoryRepository">The other investment balance history repository.</param>
        /// <param name="otherInvestmentTagService">The other investment tag service.</param>
        /// <param name="mapper">The mapper for converting between models and entities.</param>
        public OtherInvestmentService(IRepository<OtherInvestment> repository, IRepository<OtherInvestmentBalaceHistory> otherInvestmentBalaceHistoryRepository,
            IOtherInvestmentTagService otherInvestmentTagService, IMapper mapper) : base(repository, mapper)
        {
            this.otherInvestmentBalaceHistoryRepository = otherInvestmentBalaceHistoryRepository;
            this.otherInvestmentTagService = otherInvestmentTagService;
        }

        /// <summary>
        /// Adds a new other investment.
        /// </summary>
        /// <param name="model">The other investment model to add.</param>
        /// <returns>The ID of the newly added other investment.</returns>
        public override int Add(OtherInvestmentModel model)
        {
            int id = base.Add(model);
            OtherInvestmentBalaceHistory otherInvestmentBalanceHistory = new OtherInvestmentBalaceHistory();
            otherInvestmentBalanceHistory.Balance = model.OpeningBalance;
            otherInvestmentBalanceHistory.Date = model.Created;
            otherInvestmentBalanceHistory.OtherInvestmentId = id;
            otherInvestmentBalaceHistoryRepository.Create(otherInvestmentBalanceHistory);
            otherInvestmentBalaceHistoryRepository.Save();
            return id;
        }

        /// <summary>
        /// Updates an existing other investment.
        /// </summary>
        /// <param name="model">The other investment model to update.</param>
        public override void Update(OtherInvestmentModel model)
        {
            base.Update(model);
            OtherInvestmentBalaceHistory otherInvestmentBalanceHistory = FindFirstRelatedHistoryRecord(model.Id.Value);
            otherInvestmentBalanceHistory.Balance = model.OpeningBalance;
            otherInvestmentBalanceHistory.Date = model.Created;
            otherInvestmentBalaceHistoryRepository.Update(otherInvestmentBalanceHistory);
            otherInvestmentBalaceHistoryRepository.Save();
        }

        /// <summary>
        /// Deletes an existing other investment.
        /// </summary>
        /// <param name="id">The ID of the other investment to delete.</param>
        public override void Delete(int id)
        {
            IEnumerable<OtherInvestmentBalaceHistory> relatedHistoryRecords = otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).ToList();

            foreach (OtherInvestmentBalaceHistory historyRecord in relatedHistoryRecords)
                otherInvestmentBalaceHistoryRepository.Delete(historyRecord);

            OtherInvestmentTagModel otherInvestmentTagRelation = otherInvestmentTagService.Get(t => t.OtherInvestmentId == id).SingleOrDefault();

            if (otherInvestmentTagRelation != null)
                otherInvestmentTagService.Delete(otherInvestmentTagRelation.Id.Value);

            base.Delete(id);
        }

        /// <inheritdoc/>
        public IEnumerable<OtherInvestmentModel> GetAll(int userId)
            => repository.FindByCondition(i => i.UserIdentityId == userId).Select(i => mapper.Map<OtherInvestmentModel>(i));

        /// <inheritdoc/>
        public async Task<decimal> GetProgressForYears(int id, int? years = null)
        {
            decimal startBalance = repository.FindByCondition(o => o.Id == id).Single().OpeningBalance;
            decimal endBalance = otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestmentId == id)
                .OrderBy(a => a.Date)
                .Last().Balance;

            startBalance += await GetTotalyInvested(id, DateTime.MinValue, years is null ? DateTime.MinValue : DateTime.Now.AddYears(-years.Value));
            endBalance -= await GetTotalyInvested(id, years is null ? DateTime.MinValue : DateTime.Now.AddYears(-years.Value), DateTime.Now);

            if (startBalance == endBalance)
                return 0;

            if (startBalance == 0)
                startBalance = 1;

            return (endBalance / startBalance) * 100 - 100;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalyInvested(int otherinvestmentId, DateTime fromDate, DateTime toDate)
        {
            int tagId = otherInvestmentTagService.Get(p => p.OtherInvestmentId == otherinvestmentId).Select(t => t.TagId).SingleOrDefault();
            decimal totalyInvested = default;

            if (tagId != default)
                totalyInvested = (await otherInvestmentTagService.GetPaymentsForTag(otherinvestmentId, tagId))
                    .Where(p => p.Date > fromDate)
                    .Where(p => p.Date < toDate)
                    .Sum(a => a.Amount);

            return totalyInvested;
        }

        /// <inheritdoc/>
        public IEnumerable<(int otherInvestmentId, decimal totalInvested)> GetTotalyInvested(DateTime fromDate)
        {
            var data = repository.FindAll()
                .Include(t => t.OtherInvestmentTags)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.PaymentTags)
                .ThenInclude(t => t.Payment)
                .Select(d => new
                {
                    OtherInvestmentId = d.Id,
                    TotalyInvested = d.OtherInvestmentTags.Select(t => t.Tag)
                            .SelectMany(t => t.PaymentTags)
                            .Select(p => p.Payment)
                            .Where(p => p.Date > fromDate)
                            .Sum(a => a.Amount)
                });

            foreach (var item in data)
                yield return (item.OtherInvestmentId, item.TotalyInvested);
        }

        /// <inheritdoc/>
        public bool UserHasRightToPayment(int otherInvestmentId, int userId)
            => repository.FindByCondition(a => a.Id == otherInvestmentId && a.UserIdentityId == userId).Count() == 1;

        /// <inheritdoc/>
        public OtherInvestmentBalanceSummaryModel GetAllInvestmentSummary(int userId)
        {
            List<OtherInvestmentBalaceHistory> baseData = otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestment.UserIdentityId == userId).ToList();
            IEnumerable<OtherInvestmentBalaceHistoryModel> lastData = FilterBaseBalaceData(baseData, DateTime.MinValue);
            IEnumerable<OtherInvestmentBalaceHistoryModel> oneYearEarlierData = FilterBaseBalaceData(baseData, DateTime.Now.AddYears(-1));

            return new OtherInvestmentBalanceSummaryModel(lastData, oneYearEarlierData);
        }

        /// <summary>
        /// Filters the base balance data for a specific date.
        /// </summary>
        /// <param name="baseData">The base balance data.</param>
        /// <param name="balancesFrom">The date to filter from.</param>
        /// <returns>An enumerable collection of filtered balance history models.</returns>
        private IEnumerable<OtherInvestmentBalaceHistoryModel> FilterBaseBalaceData(List<OtherInvestmentBalaceHistory> baseData, DateTime balancesFrom)
        {
            IEnumerable<(int otherInvestmentId, decimal totalInvested)> totalyInvested = GetTotalyInvested(balancesFrom);
            IEnumerable<OtherInvestmentBalaceHistoryModel> filteredData = baseData.Where(o => (balancesFrom == DateTime.MinValue) || o.Date <= balancesFrom)
                .GroupBy(a => a.OtherInvestmentId)
                .Select(x => mapper.Map<OtherInvestmentBalaceHistoryModel>(x.OrderByDescending(y => y.Date).FirstOrDefault()))
                .ToList();

            filteredData = filteredData.GroupJoin(
                    totalyInvested,
                    investmentData => investmentData.OtherInvestmentId,
                    totalyInvested => totalyInvested.otherInvestmentId,
                    (investmentData, totalyInvested) => new { investmentData, totalyInvested }
                )
                .SelectMany(
                    x => x.totalyInvested.DefaultIfEmpty(),
                    (groupedData, totalyInvested) =>
                    {
                        groupedData.investmentData.Invested = totalyInvested.totalInvested;
                        return groupedData.investmentData;
                    }
                );

            return filteredData;
        }

        /// <summary>
        /// Finds the first related history record for a specific other investment.
        /// </summary>
        /// <param name="id">The ID of the other investment.</param>
        /// <returns>The first related history record.</returns>
        private OtherInvestmentBalaceHistory FindFirstRelatedHistoryRecord(int id) => otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).OrderBy(a => a.Date).First();
    }
}
