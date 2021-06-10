using System.ComponentModel.DataAnnotations;

namespace BudgetManager.ManagerWeb.Models
{
    public class UserModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}