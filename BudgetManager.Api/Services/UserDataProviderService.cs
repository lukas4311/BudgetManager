using System.Security.Claims;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace BudgetManager.Api.Services
{
    public class UserDataProviderService : IUserDataProviderService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserDataProviderService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public UserIdentification GetUserIdentification()
        {
            return new UserIdentification
            {
                UserId = int.Parse(this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = this.httpContextAccessor.HttpContext.User.Identity.Name
            };
        }
    }
}
