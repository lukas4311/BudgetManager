using System.Collections.Generic;

namespace Data.DataModels
{
    public class UserIdentity
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public UserData UserData { get; set; }

        public List<BankAccount> BankAccounts { get; set; }
    }
}
