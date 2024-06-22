using System;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Interface for a service that provides exchange rate information for Forex.
    /// </summary>
    public interface IForexService
    {
        /// <summary>
        /// Gets the current exchange rate between two currencies.
        /// </summary>
        /// <param name="fromSymbol">The currency symbol to convert from.</param>
        /// <param name="toSymbol">The currency symbol to convert to.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current exchange rate.</returns>
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);

        /// <summary>
        /// Gets the latest available exchange rate between two currencies.
        /// </summary>
        /// <param name="fromSymbol">The currency symbol to convert from.</param>
        /// <param name="toSymbol">The currency symbol to convert to.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the latest exchange rate.</returns>
        Task<double> GetExchangeRate(string fromSymbol, string toSymbol);

        /// <summary>
        /// Gets the exchange rate between two currencies at a specific date.
        /// </summary>
        /// <param name="fromSymbol">The currency symbol to convert from.</param>
        /// <param name="toSymbol">The currency symbol to convert to.</param>
        /// <param name="atDate">The date at which to get the exchange rate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the exchange rate at the specified date.</returns>
        Task<double> GetExchangeRate(string fromSymbol, string toSymbol, DateTime atDate);
    }

}