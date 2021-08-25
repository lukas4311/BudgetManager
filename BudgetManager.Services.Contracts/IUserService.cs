using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IUserService
    {
        UserIdentification Authenticate(string username, string password);

        int GetUserId(string userLogin);
    }
}
