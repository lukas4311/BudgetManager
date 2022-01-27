using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public interface IUserDtoModel<Entity> : IDtoModel<Entity> where Entity : class, IDataModel
    {
        public int UserIdentityId { get; set; }
    }
}
