namespace Data.DataModels
{
    public class UserIdentity
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }
    }
}
