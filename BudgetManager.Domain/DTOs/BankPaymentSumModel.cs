namespace BudgetManager.Domain.DTOs
{
    public class BankPaymentSumModel
    {
        public int BankAccountId { get; set; }
        public decimal Sum { get; set; }
    }
}