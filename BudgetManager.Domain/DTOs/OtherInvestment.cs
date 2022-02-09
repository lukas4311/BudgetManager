using System;

namespace BudgetManager.Domain.DTOs
{
    public class OtherInvestment
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int OpeningBalance { get; set; }

        public int UserIdentityId { get; set; }
    }
}
