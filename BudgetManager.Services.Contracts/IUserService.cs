namespace BudgetManager.Services.Contracts
{
    public interface IUserService
    {
        bool Authenticate(string username, string password);

        int GetUserId(string userLogin);
    }
}
