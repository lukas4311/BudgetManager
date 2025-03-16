using System.ComponentModel.DataAnnotations;

namespace BudgetManager.ManagerWeb.Models
{
    /// <summary>
    /// Represents the user model.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the user's login.
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}