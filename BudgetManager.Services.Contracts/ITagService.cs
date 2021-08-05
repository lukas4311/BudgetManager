using BudgetManager.ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface ITagService
    {
        void AddTagToPayment(AddTagModel tagModel);

        void DeleteTag(int tagId);

        IEnumerable<TagModel> GetPaymentTags();

        void RemoveTagFromPayment(int tagId, int paymentId);

        void UpdateAllTags(List<string> tags, int paymentId);
    }
}