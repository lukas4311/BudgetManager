using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IUserIdentityRepository" />
    public class UserIdentityRepository : Repository<UserIdentity>, IUserIdentityRepository
    {
        public UserIdentityRepository(DataContext repositoryContext) : base(repositoryContext)
        {}
    }
}
