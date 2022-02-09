using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface ITagService : IBaseService<TagModel>
    {
        void AddTagToPayment(AddTagModel tagModel);

        IEnumerable<TagModel> GetPaymentTags();

        IEnumerable<TagModel> GetPaymentsTags(int userId);

        void RemoveTagFromPayment(int tagId, int paymentId);

        void UpdateAllTags(List<string> tags, int paymentId);
    }
}