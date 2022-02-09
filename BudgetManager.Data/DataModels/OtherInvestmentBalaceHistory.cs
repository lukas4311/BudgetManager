namespace BudgetManager.Data.DataModels
{
    public class OtherInvestmentBalaceHistory
    {
        public int Id { get; set; }

        public decimal Created { get; set; }

        public int Balance { get; set; }

        public int OtherInvestmentId { get; set; }

        public OtherInvestment OtherInvestment { get; set; }
    }
}
