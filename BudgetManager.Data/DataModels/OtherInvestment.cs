using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class OtherInvestment
    {
        public int Id { get; set; }

        public decimal Created { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int OpeningBalance { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }

        public IEnumerable<OtherInvestmentBalaceHistory> OtherInvestmentBalaceHistory { get; set; }
    }
}
