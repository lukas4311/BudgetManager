using AutoMapper;
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
    public class OtherInvestmentService : BaseService<OtherInvestmentModel, OtherInvestment, IOtherInvestmentRepository>, IOtherInvestmentService
    {
        private readonly IOtherInvestmentBalaceHistoryRepository otherInvestmentBalaceHistoryRepository;
        private readonly IOtherInvestmentTagService otherInvestmentTagService;

        public OtherInvestmentService(IOtherInvestmentRepository repository, IOtherInvestmentBalaceHistoryRepository otherInvestmentBalaceHistoryRepository,
            IOtherInvestmentTagService otherInvestmentTagService, IMapper mapper) : base(repository, mapper)
        {
            this.otherInvestmentBalaceHistoryRepository = otherInvestmentBalaceHistoryRepository;
            this.otherInvestmentTagService = otherInvestmentTagService;
        }

        public override int Add(OtherInvestmentModel model)
        {
            int id = base.Add(model);
            OtherInvestmentBalaceHistory otherInvestmentBalanceHistory = new OtherInvestmentBalaceHistory();
            otherInvestmentBalanceHistory.Balance = model.OpeningBalance;
            otherInvestmentBalanceHistory.Date = model.Created;
            otherInvestmentBalanceHistory.OtherInvestmentId = id;
            this.otherInvestmentBalaceHistoryRepository.Create(otherInvestmentBalanceHistory);
            this.otherInvestmentBalaceHistoryRepository.Save();
            return id;
        }

        public override void Update(OtherInvestmentModel model)
        {
            base.Update(model);
            OtherInvestmentBalaceHistory otherInvestmentBalanceHistory = this.FindFirstRelatedHistoryRecord(model.Id.Value);
            otherInvestmentBalanceHistory.Balance = model.OpeningBalance;
            otherInvestmentBalanceHistory.Date = model.Created;
            this.otherInvestmentBalaceHistoryRepository.Update(otherInvestmentBalanceHistory);
            this.otherInvestmentBalaceHistoryRepository.Save();
        }

        public override void Delete(int id)
        {
            base.Delete(id);
            IEnumerable<OtherInvestmentBalaceHistory> relatedHistoryRecords = this.otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).ToList();

            foreach (OtherInvestmentBalaceHistory historyRecord in relatedHistoryRecords)
                this.otherInvestmentBalaceHistoryRepository.Delete(historyRecord);

            this.otherInvestmentBalaceHistoryRepository.Save();
        }

        public IEnumerable<OtherInvestmentModel> GetAll(int userId)
        {
            return this.repository
                   .FindByCondition(i => i.UserIdentityId == userId)
                   .Select(i => this.mapper.Map<OtherInvestmentModel>(i));
        }

        public async Task<decimal> GetProgressForYears(int id, int? years = null)
        {
            decimal startBalance = this.repository.FindByCondition(o => o.Id == id).Single().OpeningBalance;
            decimal endBalance = this.otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestmentId == id)
                .OrderBy(a => a.Date)
                .Last().Balance;

            startBalance += await GetTotalyInvested(id, years is null ? DateTime.MinValue : DateTime.Now.AddYears(-years.Value));

            if (startBalance == endBalance)
                return 0;

            if (startBalance == 0)
                startBalance = 1;

            return (endBalance / startBalance) * 100 - 100;
        }

        public async Task<decimal> GetTotalyInvested(int otherinvestmentId, DateTime fromDate)
        {
            int tagId = this.otherInvestmentTagService.Get(p => p.OtherInvestmentId == otherinvestmentId).Select(t => t.TagId).SingleOrDefault();
            decimal totalyInvested = default;

            if (tagId != default)
                totalyInvested = (await this.otherInvestmentTagService.GetPaymentsForTag(otherinvestmentId, tagId))
                    .Where(p => p.Date > fromDate)
                    .Sum(a => a.Amount);

            return totalyInvested;
        }

        public IEnumerable<(int otherInvestmentId, decimal totalInvested)> GetTotalyInvested(DateTime fromDate)
        {
            var data = this.repository.FindAll()
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

        public bool UserHasRightToPayment(int otherInvestmentId, int userId)
            => this.repository.FindByCondition(a => a.Id == otherInvestmentId && a.UserIdentityId == userId).Count() == 1;

        public OtherInvestmentBalanceSummaryModel GetAllInvestmentSummary(int userId)
        {
            List<OtherInvestmentBalaceHistory> baseData = this.otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestment.UserIdentityId == userId).ToList();
            IEnumerable<OtherInvestmentBalaceHistoryModel> lastData = this.FilterBaseBalaceData(baseData, DateTime.MinValue);
            IEnumerable<OtherInvestmentBalaceHistoryModel> oneYearEarlierData = this.FilterBaseBalaceData(baseData, DateTime.Now.AddYears(-1));

            return new OtherInvestmentBalanceSummaryModel(lastData, oneYearEarlierData);
        }

        private IEnumerable<OtherInvestmentBalaceHistoryModel> FilterBaseBalaceData(List<OtherInvestmentBalaceHistory> baseData, DateTime balancesFrom)
        {
            IEnumerable<(int otherInvestmentId, decimal totalInvested)> totalyInvested = this.GetTotalyInvested(balancesFrom);
            IEnumerable<OtherInvestmentBalaceHistoryModel> filteredData = baseData.Where(o => (balancesFrom == DateTime.MinValue) || o.Date <= balancesFrom)
                .GroupBy(a => a.OtherInvestmentId)
                .Select(x => this.mapper.Map<OtherInvestmentBalaceHistoryModel>(x.OrderByDescending(y => y.Date).FirstOrDefault()))
                .ToList();

            filteredData = filteredData.GroupJoin(
                    totalyInvested,
                    investmentData => investmentData.OtherInvestmentId,
                    totalyInvested => totalyInvested.otherInvestmentId,
                    (investmentData, totalyInvested) => new { investmentData, totalyInvested }
                )
                .SelectMany(
                    x => x.totalyInvested.DefaultIfEmpty(),
                    (groupedData, totalyInvested) => {
                        groupedData.investmentData.Invested = totalyInvested.totalInvested;
                        return groupedData.investmentData;
                    }
                );

            return filteredData;
        }

        private OtherInvestmentBalaceHistory FindFirstRelatedHistoryRecord(int id) => this.otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).OrderBy(a => a.Date).First();
    }
}
