using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class UserDataRepository : Repository<UserData>, IUserDataRepository
    {
        public UserDataRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
