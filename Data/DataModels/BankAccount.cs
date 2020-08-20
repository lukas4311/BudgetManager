using System.Collections.Generic;

namespace Data.DataModels
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }

        public List<Payment> Payments { get; set; }

        public List<InterestRate> InterestRates { get; set; }

        public int OpeningBalance { get; set; }
    }
}
