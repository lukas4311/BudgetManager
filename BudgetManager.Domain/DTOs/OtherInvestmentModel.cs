using System;

namespace BudgetManager.Domain.DTOs
{
    public class OtherInvestmentModel : IDtoModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int OpeningBalance { get; set; }

        public int UserIdentityId { get; set; }

        public int CurrencySymbolId { get; set; }
    }
}
