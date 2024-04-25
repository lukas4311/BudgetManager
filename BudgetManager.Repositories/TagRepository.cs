using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ITagRepository" />
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
