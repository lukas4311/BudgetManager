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
        public OtherInvestmentService(IOtherInvestmentRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public IEnumerable<OtherInvestmentModel> GetAll(int userId)
        {
            return this.repository
                   .FindByCondition(i => i.UserIdentityId == userId)
                   .Select(i => this.mapper.Map<OtherInvestmentModel>(i));
        }

        public bool UserHasRightToPayment(int otherInvestmentId, int userId) 
            => this.repository.FindByCondition(a => a.Id == otherInvestmentId && a.UserIdentityId == userId).Count() == 1;
    }
}
