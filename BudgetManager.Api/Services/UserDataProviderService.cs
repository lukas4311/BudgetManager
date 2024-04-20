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
                UserId = int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = httpContextAccessor.HttpContext.User.Identity.Name
            };
        }
    }
}
