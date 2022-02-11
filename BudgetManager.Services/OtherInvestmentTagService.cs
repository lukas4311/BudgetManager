using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class OtherInvestmentTagService : BaseService<OtherInvestmentTagModel, OtherInvestmentTag, IOtherInvestmentTagRepository>, IOtherInvestmentTagService
    {
        public OtherInvestmentTagService(IOtherInvestmentTagRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
