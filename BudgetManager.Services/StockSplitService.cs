using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class StockSplitService : BaseService<StockSplitModel, StockSplit, IStockSplitRepository>, IStockSplitService
    {
        public StockSplitService(IStockSplitRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
