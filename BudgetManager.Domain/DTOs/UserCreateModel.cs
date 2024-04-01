namespace BudgetManager.Domain.DTOs
{
    public class UserCreateModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLocked { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
