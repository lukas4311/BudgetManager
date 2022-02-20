using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public class OtherInvestmentService : BaseService<OtherInvestmentModel, OtherInvestment, IOtherInvestmentRepository>, IOtherInvestmentService
    {
        private readonly IOtherInvestmentBalaceHistoryRepository otherInvestmentBalaceHistoryRepository;

        public OtherInvestmentService(IOtherInvestmentRepository repository, IOtherInvestmentBalaceHistoryRepository otherInvestmentBalaceHistoryRepository, IMapper mapper) : base(repository, mapper)
        {
            this.otherInvestmentBalaceHistoryRepository = otherInvestmentBalaceHistoryRepository;
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

        public decimal GetProgressForYears(int id, int? years = null)
        {
            var startBalance = this.otherInvestmentBalaceHistoryRepository.FindByCondition(o => o.OtherInvestmentId == id && (years == null || o.Date < System.DateTime.Now.AddYears(-years.Value))).OrderByDescending(a => a.Date).First().Balance;
            var endBalance = this.otherInvestmentBalaceHistoryRepository.FindByCondition(o => o.OtherInvestmentId == id).OrderBy(a => a.Date).Last().Balance;

            if (startBalance == endBalance)
                return 0;

            return (decimal)((endBalance * 1.0 / startBalance * 1.0) * 100.0 - 100.0);
        }

        public bool UserHasRightToPayment(int otherInvestmentId, int userId)
            => this.repository.FindByCondition(a => a.Id == otherInvestmentId && a.UserIdentityId == userId).Count() == 1;

        private OtherInvestmentBalaceHistory FindFirstRelatedHistoryRecord(int id) => this.otherInvestmentBalaceHistoryRepository.FindByCondition(e => e.OtherInvestmentId == id).OrderBy(a => a.Date).First();
    }
}
