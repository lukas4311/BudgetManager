using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for managing other investments.
    /// </summary>
    public interface IOtherInvestmentService : IBaseService<OtherInvestmentModel, OtherInvestment>
    {
        /// <summary>
        /// Gets all other investments for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An enumerable collection of other investment models for the user.</returns>
        IEnumerable<OtherInvestmentModel> GetAll(int userId);

        /// <summary>
        /// Checks if a user has the right to access a specific payment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the investment.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user has the right to access the payment, otherwise false.</returns>
        bool UserHasRightToPayment(int otherInvestmentId, int userId);

        /// <summary>
        /// Gets the progress of an investment over a number of years.
        /// </summary>
        /// <param name="id">The ID of the investment.</param>
        /// <param name="years">The number of years to calculate progress for.</param>
        /// <returns>The progress of the investment as a percentage.</returns>
        Task<decimal> GetProgressForYears(int id, int? years = null);

        /// <summary>
        /// Gets a summary of all investments for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A summary model of all investments for the user.</returns>
        OtherInvestmentBalanceSummaryModel GetAllInvestmentSummary(int userId);

        /// <summary>
        /// Gets the total amount invested in an investment between two dates.
        /// </summary>
        /// <param name="otherinvestmentId">The ID of the investment.</param>
        /// <param name="fromDate">The start date.</param>
        /// <param name="toDate">The end date.</param>
        /// <returns>The total amount invested.</returns>
        Task<decimal> GetTotalyInvested(int otherinvestmentId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Gets the total amount invested in all investments from a specified date.
        /// </summary>
        /// <param name="fromDate">The start date.</param>
        /// <returns>An enumerable collection of tuples containing the investment ID and total amount invested.</returns>
        IEnumerable<(int otherInvestmentId, decimal totalInvested)> GetTotalyInvested(DateTime fromDate);
    }
}
