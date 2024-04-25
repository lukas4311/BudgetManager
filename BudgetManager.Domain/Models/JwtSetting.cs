namespace BudgetManager.Domain.Models;

public record JwtSetting(string SecretKey, int ExpireMinutes);