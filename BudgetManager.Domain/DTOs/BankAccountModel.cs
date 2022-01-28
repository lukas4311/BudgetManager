using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class BankAccountModel: IDtoModel<BankAccount>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int OpeningBalance { get; set; }

        public int UserIdentityId { get; set; }

        public BankAccount ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}
