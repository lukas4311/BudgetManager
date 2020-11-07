using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerWeb.Models;

namespace ManagerWeb.Services
{
    public interface IUserService
    {
        Task<UserModel> Authenticate(string username, string password);

        Task SignIn(string login, int userId);

        int GetUserId(string userLogin);
    }
}