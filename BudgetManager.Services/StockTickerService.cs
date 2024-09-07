using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Enums;
using BudgetManager.Domain.Models;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
            => mapper.Map<StockTickerModel>(repository.FindAll().Include(t => t.EnumItemType).Where(t => t.EnumItemType.Code == nameof(EEnumTypes.StockTradeTickers) && t.Id == id));

        /// <inheritdoc/>
        public IEnumerable<StockTickerModel> GetAll() 
            => repository.FindAll().Include(t => t.EnumItemType).Where(t => t.EnumItemType.Code == nameof(EEnumTypes.StockTradeTickers)).Select(t => mapper.Map<StockTickerModel>(t));

        /// <inheritdoc/>
        public IEnumerable<string> GetAllAvailableTickersForPriceSearch()
        {
            var tickers = GetAll().Select(t => t.Ticker);
            var tickersMetadata = tickers.Where(t => !string.IsNullOrEmpty(t)).Select(m => JsonSerializer.Deserialize<TickerMetadata>(m));
            IEnumerable<string> allAvailableTickerForPrice = tickers.Union(tickersMetadata.Where(t => !string.IsNullOrEmpty(t.PriceTicker)).Select(t => t.PriceTicker));
            return allAvailableTickerForPrice;
        }

        /// <inheritdoc/>
        public void UpdateTickerMetadata(int tickerId, string metadata)
        {
            EnumItem ticker = repository.Get(tickerId);
            ticker.Metadata = metadata;
            repository.Update(ticker);
            repository.Save();
        }

        /// <inheritdoc/>
        public void UpdateTicker(StockTickerModel stockTickerModel)
        {
            EnumItem ticker = repository.Get(stockTickerModel.Id.Value);
            ticker.Code = stockTickerModel.Ticker;
            ticker.Name = stockTickerModel.Name;
            ticker.Metadata = stockTickerModel.Metadata;
            repository.Update(ticker);
            repository.Save();
        }
    }
}
