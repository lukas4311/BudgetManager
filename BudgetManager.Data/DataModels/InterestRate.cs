using System;

namespace BudgetManager.Data.DataModels
{
    public class InterestRate : IDataModel
    {
        public int Id { get; set; }

        public decimal RangeFrom { get; set; }

        public decimal? RangeTo { get; set; }

        public decimal Value { get; set; }

        public int BankAccountId { get; set; }

        public BankAccount BankAccount { get; set; }

        public DateTime PayoutDate { get; set; }
    }
}
