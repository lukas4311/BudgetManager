using ManagerWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IUserIdentityRepository userIdentityRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userIdentityRepository = userIdentityRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserModel> Authenticate(string username, string passwordHash)
        {
            return await Task.Run(() => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == passwordHash).Select(a => new UserModel { Login = a.Login }).SingleOrDefault()).ConfigureAwait(false);
        }

        public int GetUserId(string userLogin)
        {
            return this.userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;
        }

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
