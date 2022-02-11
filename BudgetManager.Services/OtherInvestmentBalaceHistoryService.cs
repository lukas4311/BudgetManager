using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class OtherInvestmentBalaceHistoryService : BaseService<OtherInvestmentBalaceHistoryModel, OtherInvestmentBalaceHistory, IOtherInvestmentBalaceHistoryRepository>, IOtherInvestmentBalaceHistoryService
    {
        public OtherInvestmentBalaceHistoryService(IOtherInvestmentBalaceHistoryRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
