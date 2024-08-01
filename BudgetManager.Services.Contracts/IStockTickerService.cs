using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
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
    }
}
