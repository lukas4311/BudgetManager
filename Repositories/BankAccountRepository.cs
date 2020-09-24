using Data;
using Data.DataModels;

namespace Repository
{
    public class BankAccountRepository : Repository<InterestRate>, IBankAccountRepository
    {
        public BankAccountRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
