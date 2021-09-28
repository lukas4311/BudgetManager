using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
