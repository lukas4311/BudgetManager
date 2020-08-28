using Data;
using Data.DataModels;

namespace Repository
{
    public class UserIdentityRepository : Repository<UserIdentity>, IUserIdentityRepository
    {
        public UserIdentityRepository(DataContext repositoryContext) : base(repositoryContext)
        {}
    }
}
