using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class StockTickerService : IStockTickerService
    {
        private readonly IRepository<EnumItem> repository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTickerService"/> class.
        /// </summary>
        /// <param name="repository">The repository for stock tickers.</param>
        /// <param name="mapper">The mapper for mapping between models.</param>
        public StockTickerService(IRepository<EnumItem> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public StockTickerModel Get(int id)
        {
            return mapper.Map<StockTickerModel>(repository.Get(id));
        }

        /// <inheritdoc/>
        public IEnumerable<StockTickerModel> GetAll()
        {
            return repository.FindAll().Select(t => mapper.Map<StockTickerModel>(t));
        }
    }
}
