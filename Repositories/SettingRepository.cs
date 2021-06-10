using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
