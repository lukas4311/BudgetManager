using System.Collections.Generic;
using System.Security.Claims;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Models;

namespace BudgetManager.Services.Contracts
{
    public interface IJwtService
    {
        bool IsTokenValid(string token);

        string GenerateToken(UserIdentification model);

        IEnumerable<Claim> GetTokenClaims(string token);

        UserIdentification GetUserIdentification(string token);

        void SetUp(JwtSetting jwtSetting);
    }
}
