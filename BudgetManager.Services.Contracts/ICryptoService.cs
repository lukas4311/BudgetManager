﻿using System;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetManager.InfluxDbData;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for handling cryptocurrency-related operations.
    /// </summary>
    public interface ICryptoService : IBaseService<TradeHistory, CryptoTradeHistory, Repository.IRepository<ComodityTradeHistory>>
    {
        /// <summary>
        /// Retrieves cryptocurrency trade history for a user based on their login.
        /// </summary>
        /// <param name="userLogin">The user's login.</param>
        /// <returns>A collection of <see cref="TradeHistory"/> objects.</returns>
        IEnumerable<TradeHistory> GetByUser(string userLogin);

        /// <summary>
        /// Retrieves cryptocurrency trade history for a user based on their ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of <see cref="TradeHistory"/> objects.</returns>
        IEnumerable<TradeHistory> GetByUser(int userId);

        /// <summary>
        /// Retrieves a specific cryptocurrency trade history entry for a user.
        /// </summary>
        /// <param name="id">The trade history entry ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>The <see cref="TradeHistory"/> object.</returns>
        TradeHistory Get(int id, int userId);

        /// <summary>
        /// Checks if a user has rights to a specific cryptocurrency trade.
        /// </summary>
        /// <param name="cryptoTradeId">The crypto trade identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if the user has rights to the crypto trade; otherwise, <c>false</c>.</returns>
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);

        /// <summary>
        /// Asynchronously retrieves the current exchange rate between two symbols.
        /// </summary>
        /// <param name="fromSymbol">The symbol to convert from.</param>
        /// <param name="toSymbol">The symbol to convert to.</param>
        /// <returns>The current exchange rate.</returns>
        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);

        /// <summary>
        /// Asynchronously retrieves the exchange rate between two symbols at a specific date.
        /// </summary>
        /// <param name="fromSymbol">The symbol to convert from.</param>
        /// <param name="toSymbol">The symbol to convert to.</param>
        /// <param name="atDate">The date to retrieve the exchange rate for.</param>
        /// <returns>The exchange rate at the specified date.</returns>
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol, DateTime atDate);

        /// <summary>
        /// Asynchronously retrieves the price history of a cryptocurrency.
        /// </summary>
        /// <param name="ticker">The ticker symbol of the cryptocurrency.</param>
        /// <returns>A collection of <see cref="CryptoDataV2"/> objects representing the price history.</returns>
        Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker);

        /// <summary>
        /// Asynchronously retrieves the price history of a cryptocurrency starting from a specific date.
        /// </summary>
        /// <param name="ticker">The ticker symbol of the cryptocurrency.</param>
        /// <param name="from">The start date for the price history.</param>
        /// <returns>A collection of <see cref="CryptoDataV2"/> objects representing the price history.</returns>
        Task<CryptoDataV2> GetCryptoPriceAtDate(string ticker, DateTime atDate);

        /// <summary>
        /// Asynchronously retrieves the price of a cryptocurrency at a specific date.
        /// </summary>
        /// <param name="ticker">The ticker symbol of the cryptocurrency.</param>
        /// <param name="atDate">The date to retrieve the price for.</param>
        /// <returns>The <see cref="CryptoDataV2"/> object representing the price at the specified date.</returns>
        Task<IEnumerable<CryptoDataV2>> GetCryptoPriceHistory(string ticker, DateTime from);

        /// <summary>
        /// Stores a broker report to process for a user.
        /// </summary>
        /// <param name="brokerFileData">The broker file data in byte array format.</param>
        /// <param name="userId">The user ID.</param>
        void StoreReportToProcess(byte[] brokerFileData, int userId);
    }
}