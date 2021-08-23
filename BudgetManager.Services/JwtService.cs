using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace BudgetManager.Services
{
    public record JwtSetting(string SecretKey, string SecurityAlgorithm, int ExpireMinutes);

    public class JwtService : IJwtService
    {
        private readonly JwtSetting jwtSetting;

        public JwtService(JwtSetting jwtSetting)
        {
            this.jwtSetting = jwtSetting;
        }

        public string GenerateToken(UserIdentification model)
        {
            if (model is null)
                throw new ArgumentException("Arguments to create token are not valid.");

            Claim[] claims = new Claim[2];
            claims[0] = new Claim(ClaimTypes.NameIdentifier, value: model.UserId.ToString());
            claims[1] = new Claim(ClaimTypes.Name, value: model.UserName);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(this.jwtSetting.ExpireMinutes)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), this.jwtSetting.SecurityAlgorithm)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(this.jwtSetting.SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters() =>
             new TokenValidationParameters()
             {
                 ValidateIssuer = false,
                 ValidateAudience = false,
                 IssuerSigningKey = GetSymmetricSecurityKey()
             };
    }
}
