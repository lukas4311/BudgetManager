using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface ITagService
    {
        void AddTagToPayment(AddTagModel tagModel);

        void Delete(int tagId);

        IEnumerable<TagModel> GetPaymentTags();

        void RemoveTagFromPayment(int tagId, int paymentId);

        void UpdateAllTags(List<string> tags, int paymentId);
    }
}