namespace ManagerWeb.Models.DTOs
{
    internal class BankBalanceModel
    {
        public int Id { get; set; }

        public int OpeningBalance { get; set; }

        public decimal Balance { get; set; }
    }
}
