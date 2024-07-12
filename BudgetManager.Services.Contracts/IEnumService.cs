using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;

namespace BudgetManager.Services.Contracts
{
    public interface IEnumService : IBaseService<EnumItemModelAdjusted, EnumItem, IRepository<EnumItem>>
    {
    }
}
