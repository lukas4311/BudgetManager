using System.Collections.Generic;
using System.Security.Claims;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Models;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for managing JSON Web Tokens (JWT).
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Checks if a specified JWT token is valid.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>True if the token is valid, otherwise false.</returns>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        bool IsTokenValid(string token);

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="model">The user identification model containing user details.</param>
        /// <returns>A JWT token as a string.</returns>
        /// <exception cref="ArgumentException">Thrown when the user model is null.</exception>
        string GenerateToken(UserIdentification model);

        /// <summary>
        /// Gets the claims from a specified JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>An enumerable collection of claims from the token.</returns>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        /// <exception cref="InvalidToken">Thrown when the token is not valid.</exception>
        IEnumerable<Claim> GetTokenClaims(string token);

        /// <summary>
        /// Gets the user identification information from a specified JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>The user identification information extracted from the token.</returns>
        UserIdentification GetUserIdentification(string token);

        /// <summary>
        /// Sets up the JWT service with the specified settings.
        /// </summary>
        /// <param name="jwtSetting">The settings for JWT.</param>
        void SetUp(JwtSetting jwtSetting);
    }
}
