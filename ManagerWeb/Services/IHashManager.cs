namespace ManagerWeb.Services
{
    internal interface IHashManager
    {
        string HashPasswordToSha512(string password);
    }
}
