using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Represents a service for managing tags.
    /// </summary>
    public interface ITagService : IBaseService<TagModel, Tag, IRepository<Tag>>
    {
        /// <summary>
        /// Adds a tag to a payment.
        /// </summary>
        /// <param name="tagModel">The tag model containing payment information.</param>
        void AddTagToPayment(AddTagModel tagModel);

        /// <summary>
        /// Gets the payment tags associated with user identities.
        /// </summary>
        /// <returns>An enumerable of <see cref="TagModel"/>.</returns>
        IEnumerable<TagModel> GetPaymentTags();

        /// <summary>
        /// Gets the payment tags associated with a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>An enumerable of <see cref="TagModel"/>.</returns>
        IEnumerable<TagModel> GetPaymentsTags(int userId);

        /// <summary>
        /// Removes a tag from a payment.
        /// </summary>
        /// <param name="tagId">The tag ID.</param>
        /// <param name="paymentId">The payment ID.</param>
        void RemoveTagFromPayment(int tagId, int paymentId);

        /// <summary>
        /// Updates tags for a payment.
        /// </summary>
        /// <param name="tags">The list of tags to update.</param>
        /// <param name="paymentId">The payment ID.</param>
        void UpdateAllTags(List<string> tags, int paymentId);
    }
}