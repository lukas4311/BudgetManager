using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModels
{
    public class UserData
    {
        public int Id { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLocked { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
