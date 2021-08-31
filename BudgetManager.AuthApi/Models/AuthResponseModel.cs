namespace BudgetManager.AuthApi.Models
{
    public record AuthResponseModel(string Token, int UserId, string UserName);
}
