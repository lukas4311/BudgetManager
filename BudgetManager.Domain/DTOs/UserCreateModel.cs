using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Domain.DTOs
{
    public class UserCreateModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Phone { get; set; }
    }
}
