using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class UserIdentityRepository : Repository<UserIdentity>, IUserIdentityRepository
    {
        public UserIdentityRepository(DataContext repositoryContext) : base(repositoryContext)
        {}
    }
}
