using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public interface IDtoModel<Entity> where Entity : class, IDataModel
    {
        public int Id { get; set; }

        public Entity ToEntity();
    }
}
