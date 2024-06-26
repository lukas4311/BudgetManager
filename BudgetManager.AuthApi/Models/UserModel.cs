namespace BudgetManager.AuthApi.Models
{
    /// <summary>
    /// User model
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User login
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User pass
        /// </summary>
        public string Password { get; set; }
    }
}
