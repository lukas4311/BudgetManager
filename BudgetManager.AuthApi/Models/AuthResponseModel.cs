namespace BudgetManager.AuthApi.Models
{
    /// <summary>
    /// Auth response model
    /// </summary>
    /// <param name="Token">Access token</param>
    /// <param name="UserId">User ID</param>
    /// <param name="UserName">User name</param>
    public record AuthResponseModel(string Token, int UserId, string UserName);
}
