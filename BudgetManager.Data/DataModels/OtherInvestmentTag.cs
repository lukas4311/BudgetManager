namespace BudgetManager.Data.DataModels
{
    public class OtherInvestmentTag
    {
        public int Id { get; set; }

        public int OtherInvestmentId { get; set; }

        public OtherInvestment OtherInvestment { get; set; }

        public int TagId { get; set; }

        public Tag Tag { get; set; }
    }
}