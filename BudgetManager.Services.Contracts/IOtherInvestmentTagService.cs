using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for managing operations related to tags associated with other investments.
    /// </summary>
    public interface IOtherInvestmentTagService : IBaseService<OtherInvestmentTagModel, OtherInvestmentTag>
    {
        /// <summary>
        /// Retrieves all payments associated with a specific tag and other investment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the other investment.</param>
        /// <param name="tagId">The ID of the tag.</param>
        /// <returns>A collection of payments associated with the tag and other investment.</returns>
        Task<IEnumerable<PaymentModel>> GetPaymentsForTag(int otherInvestmentId, int tagId);

        /// <summary>
        /// Replaces the tag associated with an other investment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the other investment.</param>
        /// <param name="tagId">The ID of the new tag to associate.</param>
        /// <returns>The ID of the newly added other investment tag.</returns>
        int ReplaceTagForOtherInvestment(int otherInvestmentId, int tagId);
    }
}
