﻿using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Stock ticker service.
    /// </summary>
    public interface IStockTickerService
    {
        /// <summary>
        /// Get all stock tickers
        /// </summary>
        /// <returns>List of stock tickers</returns>
        IEnumerable<StockTickerModel> GetAll();

        /// <summary>
        /// Get ticker with Id
        /// </summary>
        /// <param name="id">Ticker id</param>
        /// <returns>Ticker model with specified Id</returns>
        StockTickerModel Get(int id);

        /// <summary>
        /// Method to update ticker metadata
        /// </summary>
        /// <param name="tickerId">Ticker id</param>
        /// <param name="metadata">Metadata</param>
        void UpdateTickerMetadata(int tickerId, string metadata);

        /// <summary>
        /// Update ticker
        /// </summary>
        /// <param name="stockTickerModel">Stock ticker model</param>
        void UpdateTicker(StockTickerModel stockTickerModel);

        /// <summary>
        /// Get all tickers for price search
        /// </summary>
        /// <returns>List of tickers for price search</returns>
        IEnumerable<string> GetAllAvailableTickersForPriceSearch();
    }
}
