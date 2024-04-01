using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IUserService
    {
        /// <summary>
        /// Authentication of user
        /// </summary>
        /// <param name="username">User login</param>
        /// <param name="password">User password</param>
        /// <returns>User model when user is authenticated</returns>
        UserIdentification Authenticate(string username, string password);

        /// <summary>
        /// Get user id for user login
        /// </summary>
        /// <param name="userLogin">User login</param>
        /// <returns>Id of found user</returns>
        int GetUserId(string userLogin);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userCreateModel">User data model</param>
        void CreateUser(UserCreateModel userCreateModel);
    }
}
