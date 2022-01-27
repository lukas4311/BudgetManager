using BudgetManager.Data.DataModels;
using System;

namespace BudgetManager.Domain.DTOs
{
    public class BudgetModel : IUserDtoModel<Budget>
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int Amount { get; set; }

        public string Name { get; set; }

        public int UserIdentityId { get; set; }

        public Budget ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
