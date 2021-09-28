namespace BudgetManager.Data.DataModels
{
    public class PaymentTag
    {
        public int Id { get; set; }

        public int PaymentId { get; set; }

        public Payment Payment { get; set; }

        public int TagId { get; set; }

        public Tag Tag { get; set; }
    }
}
