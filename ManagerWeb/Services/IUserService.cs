using System.Threading.Tasks;
using BudgetManager.ManagerWeb.Models;

namespace BudgetManager.ManagerWeb.Services
{
    /// <summary>
    /// Provides user-related services such as authentication and sign-in.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates the user with the given username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the authenticated user model.</returns>
        Task<UserModel> Authenticate(string username, string password);

        /// <summary>
        /// Signs in the user with the specified login and user ID.
        /// </summary>
        /// <param name="login">The login of the user.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SignIn(string login, int userId);

        /// <summary>
        /// Gets the user ID for the specified user login.
        /// </summary>
        /// <param name="userLogin">The login of the user.</param>
        /// <returns>The ID of the user.</returns>
        int GetUserId(string userLogin);
    }
}