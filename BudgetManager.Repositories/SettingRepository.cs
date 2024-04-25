using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ISettingRepository" />
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
