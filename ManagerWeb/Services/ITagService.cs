using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    public interface ITagService
    {
        void AddTag(TagModel tagModel);
        void DeleteTag(int tagId);
        IEnumerable<TagModel> GetPaymentTags();
    }
}