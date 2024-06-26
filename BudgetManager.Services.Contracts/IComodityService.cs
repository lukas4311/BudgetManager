using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    /// <summary>
    /// Service class for handling commodity-related operations.
    /// </summary>
    public interface IComodityService : IBaseService<ComodityTradeHistoryModel, ComodityTradeHistory, IRepository<ComodityTradeHistory>>
    {
        /// <summary>
        /// Retrieves commodity trade history for a user based on their login.
        /// </summary>
        /// <param name="userLogin">The user's login.</param>
        /// <returns>A collection of <see cref="ComodityTradeHistoryModel"/> objects.</returns>
        IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin);

        /// <summary>
        /// Retrieves commodity trade history for a user based on their ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of <see cref="ComodityTradeHistoryModel"/> objects.</returns>
        IEnumerable<ComodityTradeHistoryModel> GetByUser(int userId);

        /// <summary>
        /// Retrieves all commodity types.
        /// </summary>
        /// <returns>A collection of <see cref="ComodityTypeModel"/> objects.</returns>
        IEnumerable<ComodityTypeModel> GetComodityTypes();

        /// <summary>
        /// Retrieves all commodity units.
        /// </summary>
        /// <returns>A collection of <see cref="ComodityUnitModel"/> objects.</returns>
        IEnumerable<ComodityUnitModel> GetComodityUnits();

        /// <summary>
        /// Asynchronously retrieves the current gold price per ounce.
        /// </summary>
        /// <returns>The current gold price per ounce.</returns>
        Task<double> GetCurrentGoldPriceForOunce();

        /// <summary>
        /// Checks if a user has rights to a specific crypto trade.
        /// </summary>
        /// <param name="cryptoTradeId">The crypto trade identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if the user has rights to the crypto trade; otherwise, <c>false</c>.</returns>
        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);
    }
}