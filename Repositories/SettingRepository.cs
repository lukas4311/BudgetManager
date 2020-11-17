using Data;
using Data.DataModels;

namespace Repository
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
