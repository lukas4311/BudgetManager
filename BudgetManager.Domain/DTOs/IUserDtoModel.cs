﻿using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public interface IUserDtoModel : IDtoModel
    {
        public int UserIdentityId { get; set; }
    }
}
