﻿using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Services
{
    public interface ITagService
    {
        void AddTagToPayment(AddTagModel tagModel);

        void DeleteTag(int tagId);

        IEnumerable<TagModel> GetPaymentTags();

        void RemoveTagFromPayment(int tagId, int paymentId);
    }
}