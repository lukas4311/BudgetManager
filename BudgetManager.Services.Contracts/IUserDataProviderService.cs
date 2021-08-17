using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IUserDataProviderService
    {
        UserIdentification GetUserIdentification();
    }
}
