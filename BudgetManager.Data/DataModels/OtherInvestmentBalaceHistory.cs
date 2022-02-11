using System;

namespace BudgetManager.Data.DataModels
{
    public class OtherInvestmentBalaceHistory : IDataModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Balance { get; set; }

        public int OtherInvestmentId { get; set; }

        public OtherInvestment OtherInvestment { get; set; }
    }
}
