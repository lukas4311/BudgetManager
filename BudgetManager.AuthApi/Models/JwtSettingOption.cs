namespace BudgetManager.AuthApi.Models
{
    public class JwtSettingOption
    {
        public string Secret { get; set; }

        public int Expiration { get; set; }
    }
}
