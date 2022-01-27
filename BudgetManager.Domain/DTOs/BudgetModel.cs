using System;

namespace BudgetManager.Domain.DTOs
{
    public class BudgetModel : IUserDtoModel
    {
        public int? Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public int Amount { get; set; }

        public string Name { get; set; }

        public int UserIdentityId { get; set; }
    }
}
