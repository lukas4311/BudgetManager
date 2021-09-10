using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BudgetManager.Api.Extensions
{
    public static class HttpContextExtension
    {
        internal static int GetUserId(this HttpContext httpContext) => int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        internal static string GetUserName(this HttpContext httpContext) => httpContext.User.FindFirst(ClaimTypes.Name).Value;
    }
}
