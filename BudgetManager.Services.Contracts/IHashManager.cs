namespace BudgetManager.Services.Contracts
{
    public interface IHashManager
    {
        string HashPasswordToSha512(string password);
    }
}
