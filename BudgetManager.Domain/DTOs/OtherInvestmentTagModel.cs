namespace BudgetManager.Domain.DTOs
{
    public class OtherInvestmentTagModel : IDtoModel
    {
        public int Id { get; set; }

        public int OtherInvestmentId { get; set; }

        public int TagId { get; set; }
    }
}
