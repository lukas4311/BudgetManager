using System.Collections.Generic;
using System.Security.Claims;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IJwtService
    {
        bool IsTokenValid(string token);

        string GenerateToken(UserIdentification model);

        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
