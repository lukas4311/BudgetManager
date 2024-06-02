namespace BudgetManager.Domain.Models;

/// <summary>
/// Model for JWT setting
/// </summary>
/// <param name="SecretKey">Secure key</param>
/// <param name="ExpireMinutes">Expiration period in minutes</param>
public record JwtSetting(string SecretKey, int ExpireMinutes);