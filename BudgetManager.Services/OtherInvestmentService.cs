using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class OtherInvestmentService : BaseService<OtherInvestmentModel, OtherInvestment, IOtherInvestmentRepository>, IOtherInvestmentService
    {
        public OtherInvestmentService(IOtherInvestmentRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
