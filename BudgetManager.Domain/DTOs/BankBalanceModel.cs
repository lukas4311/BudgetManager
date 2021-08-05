namespace BudgetManager.Domain.DTOs
{
    public class BankBalanceModel
    {
        public int Id { get; set; }

        public int OpeningBalance { get; set; }

        public decimal Balance { get; set; }
    }
}
