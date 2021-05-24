using Data;
using Data.DataModels;

namespace Repository
{
    public class InterestRateRepository : Repository<InterestRate>, IInterestRateRepository
    {
        public InterestRateRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
