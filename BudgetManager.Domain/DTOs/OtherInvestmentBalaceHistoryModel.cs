using System;

namespace BudgetManager.Domain.DTOs
{
    public class OtherInvestmentBalaceHistoryModel : IDtoModel
    {
        public int? Id { get; set; }

        public DateTime Date { get; set; }

        public int Balance { get; set; }

        public decimal? Invested { get; set; }

        public int OtherInvestmentId { get; set; }
    }
}
