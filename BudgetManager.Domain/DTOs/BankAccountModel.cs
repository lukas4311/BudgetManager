namespace BudgetManager.Domain.DTOs
{
    public class BankAccountModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int OpeningBalance { get; set; }

        public int UserIdentityId { get; set; }
    }
}
