using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class StockTickerService : BaseService<StockTickerModel, StockTicker, IRepository<StockTicker>>, IStockTickerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockTickerService"/> class.
        /// </summary>
        /// <param name="repository">The repository for stock tickers.</param>
        /// <param name="mapper">The mapper for mapping between models.</param>
        public StockTickerService(IRepository<StockTicker> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
