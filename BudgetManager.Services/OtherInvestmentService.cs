using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
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
            decimal startBalance = this.otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestmentId == id && (years == null || o.Date > DateTime.Now.AddYears(-years.Value)))
                .OrderBy(a => a.Date)
                .First().Balance;

            decimal endBalance = this.otherInvestmentBalaceHistoryRepository
                .FindByCondition(o => o.OtherInvestmentId == id)
                .OrderBy(a => a.Date)
                .Last().Balance;

            var tagId = this.otherInvestmentTagService.Get(p => p.OtherInvestmentId == id).Select(t => t.TagId).SingleOrDefault();
            var payments = new List<PaymentModel>();

            if (tagId != default)
            {
                payments = (await this.otherInvestmentTagService.GetPaymentsForTag(id, tagId)).Where(p => years == null || p.Date > DateTime.Now.AddYears(-years.Value)).ToList();
                endBalance -= payments.Sum(a => a.Amount);
            }

            if (startBalance == endBalance)
                return 0;

            return (endBalance / startBalance) * 100 - 100;
        }

        public bool UserHasRightToPayment(int otherInvestmentId, int userId)
            => this.repository.FindByCondition(a => a.Id == otherInvestmentId && a.UserIdentityId == userId).Count() == 1;

        public IEnumerable<OtherInvestmentBalaceHistoryModel> GetAllInvestmentLastBalance() => this.otherInvestmentBalaceHistoryRepository.FindAll()
                .GroupBy(a => a.OtherInvestmentId)
                .Select(x => this.mapper.Map<OtherInvestmentBalaceHistoryModel>(x.OrderByDescending(y => y.Date).FirstOrDefault()))
                .ToList();

        private OtherInvestmentBalaceHistory FindFirstRelatedHistoryRecord(int id) => this.otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).OrderBy(a => a.Date).First();
    }
}
