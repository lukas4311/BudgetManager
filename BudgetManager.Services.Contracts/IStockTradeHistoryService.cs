using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Domain.Enums;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for managing operations related to stock trade history.
    /// </summary>
    public interface IStockTradeHistoryService : IBaseService<StockTradeHistoryModel, Trade, IRepository<Trade>>
    {
        /// <summary>
        /// Retrieves all stock trade history for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An enumerable of stock trade history records.</returns>
        IEnumerable<StockTradeHistoryGetModel> GetAll(int userId);

        /// <summary>
        /// Checks if a user has the right to access a specific stock trade history record.
        /// </summary>
        /// <param name="stockTradeHistoryId">The ID of the stock trade history record.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user has access; otherwise, false.</returns>
        bool UserHasRightToStockTradeHistory(int stockTradeHistoruId, int userId);

        /// <summary>
        /// Retrieves historical stock price data for a specific stock ticker.
        /// </summary>
        /// <param name="ticker">The stock ticker symbol.</param>
        /// <returns>An enumerable of historical stock price records.</returns>
        Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker);

        /// <summary>
        /// Retrieves historical stock price data for a specific stock ticker starting from a specified date.
        /// </summary>
        /// <param name="ticker">The stock ticker symbol.</param>
        /// <param name="from">The starting date from which to retrieve data.</param>
        /// <returns>An enumerable of historical stock price records.</returns>
        Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker, DateTime from);

        /// <summary>
        /// Retrieves stock trade history for a user with a specific stock ticker.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="stockTicker">The stock ticker symbol.</param>
        /// <returns>An enumerable of stock trade history records for the specified stock ticker.</returns>
        IEnumerable<StockTradeHistoryGetModel> GetTradeHistory(int userId, string stockTicker);

        /// <summary>
        /// Retrieves the latest stock price data for a specific stock ticker at a given date.
        /// </summary>
        /// <param name="ticker">The stock ticker symbol.</param>
        /// <param name="atDate">The date for which to retrieve the stock price.</param>
        /// <returns>The stock price record.</returns>
        Task<StockPrice> GetStockPriceAtDate(string ticker, DateTime atDate);

        /// <summary>
        /// Retrieves the latest stock price data for multiple stock tickers at a given date.
        /// </summary>
        /// <param name="tickers">The array of stock ticker symbols.</param>
        /// <param name="date">The date for which to retrieve the stock prices.</param>
        /// <returns>An enumerable of latest stock price records.</returns>
        Task<IEnumerable<StockPrice>> GetStocksPriceAtDate(string[] tickers, DateTime date);

        /// <summary>
        /// Retrieves all stock trade history for a user with specified currency symbol conversion.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="currencySymbol">The desired currency symbol for conversion.</param>
        /// <returns>An enumerable of stock trade history records with converted values.</returns>
        Task<IEnumerable<StockTradeHistoryGetModel>> GetAll(int userId, ECurrencySymbol currencySymbol);

        /// <summary>
        /// Stores broker report data to process.
        /// </summary>
        /// <param name="brokerFileData">The byte array of broker file data.</param>
        /// <param name="userId">The ID of the user submitting the report.</param>
        /// <param name="brokerId">The ID of broker of the report.</param>
        void StoreReportToProcess(byte[] brokerFileData, int userId, int brokerId);

        /// <summary>
        /// Get all stock trades grouped to months
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns></returns>
        IEnumerable<TradesGroupedMonth> GetAllTradesGroupedByMonth(int userId);

        /// <summary>
        /// Get all stock trades grouped by ticker
        /// </summary>
        /// <returns></returns>
        IEnumerable<TradeGroupedTicker> GetAllTradesGroupedByTicker();

        /// <summary>
        /// Get all stock trades grouped by trade date
        /// </summary>
        /// <returns></returns>
        IEnumerable<TradeGroupedTradeTime> GetAllTradesGroupedByTradeDate();
    }
}
