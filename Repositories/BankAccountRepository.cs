using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository
    {
        public BankAccountRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
