namespace Data.DataModels
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int UserDataId { get; set; }

        public UserData UserData { get; set; }
    }
}
