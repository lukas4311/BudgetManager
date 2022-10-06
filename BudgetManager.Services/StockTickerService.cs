using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class StockTickerService : BaseService<StockTickerModel, StockTicker, IStockTickerRepository>, IStockTickerService
    {
        public StockTickerService(IStockTickerRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
