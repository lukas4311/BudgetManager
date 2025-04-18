﻿using BudgetManager.ManagerWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using BudgetManager.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BudgetManager.Data.DataModels;

namespace BudgetManager.ManagerWeb.Services
{
    /// <inheritdoc />
    internal class UserService : IUserService
    {
        private readonly IRepository<UserIdentity> userIdentityRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userIdentityRepository">The repository for user identity data.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public UserService(IRepository<UserIdentity> userIdentityRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userIdentityRepository = userIdentityRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public async Task<UserModel> Authenticate(string username, string passwordHash)
        {
            return await Task.Run(() => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == passwordHash)
                        .Select(a => new UserModel { Login = a.Login }).SingleOrDefault()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public int GetUserId(string userLogin)
        {
            return this.userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;
        }

        /// <inheritdoc />
        public async Task SignIn(string login, int userId)
        {
            List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Name, login)
                    };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await this.httpContextAccessor.HttpContext.SignInAsync(principal).ConfigureAwait(false);
        }
    }
}
