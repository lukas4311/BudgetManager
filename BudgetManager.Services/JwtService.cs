using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Exceptions;
using BudgetManager.Domain.Models;
using BudgetManager.Services.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class JwtService : IJwtService
    {
        private JwtSetting jwtSetting;

        /// <inheritdoc/>
        public void SetUp(JwtSetting jwtSetting) => this.jwtSetting = jwtSetting;

        /// <inheritdoc/>
        public string GenerateToken(UserIdentification model)
        {
            CheckSettingIsSettedUp();

            if (model is null)
                throw new ArgumentException("Arguments to create token are not valid.");

            Claim[] claims = new Claim[2];
            claims[0] = new Claim(ClaimTypes.NameIdentifier, value: model.UserId.ToString());
            claims[1] = new Claim(ClaimTypes.Name, value: model.UserName);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSetting.ExpireMinutes)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        /// <inheritdoc/>
        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            CheckSettingIsSettedUp();

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
                throw new InvalidToken("Token is not valid.", ex);
            }
        }

        /// <inheritdoc/>
        public UserIdentification GetUserIdentification(string token)
        {
            IEnumerable<Claim> claims = GetTokenClaims(token);

            return new UserIdentification()
            {
                UserId = int.Parse(claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                UserName = claims.Single(c => c.Type == ClaimTypes.Name).Value
            };
        }

        /// <inheritdoc/>
        public bool IsTokenValid(string token)
        {
            CheckSettingIsSettedUp();

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

        /// <summary>
        /// Gets the symmetric security key from the JWT settings.
        /// </summary>
        /// <returns>The symmetric security key.</returns>
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(jwtSetting.SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        /// <summary>
        /// Checks if the JWT settings have been set up.
        /// </summary>
        /// <exception cref="NotSettedUpException">Thrown when the JWT settings have not been set up.</exception>
        private void CheckSettingIsSettedUp()
        {
            if (jwtSetting is null)
                throw new NotSettedUpException();
        }

        /// <summary>
        /// Gets the token validation parameters using the symmetric security key.
        /// </summary>
        /// <returns>The token validation parameters.</returns>
        private TokenValidationParameters GetTokenValidationParameters() =>
             new TokenValidationParameters()
             {
                 ValidateIssuer = false,
                 ValidateAudience = false,
                 IssuerSigningKey = GetSymmetricSecurityKey()
             };
    }
}
