using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class UserDataRepository : Repository<UserData>, IUserDataRepository
    {
        public UserDataRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
