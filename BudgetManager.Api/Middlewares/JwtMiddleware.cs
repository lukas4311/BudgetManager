using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BudgetManager.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BudgetManager.Api.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AuthApiSetting appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthApiSetting> appSettings, IHttpClientFactory httpClientFactory)
        {
            this.next = next;
            this.appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (await HasAllowAnonymousAtrribute(context) || context.Request.Path.ToString().ToLower().Contains("/scalar"))
            {
                await next(context);
                return;
            }

            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token is null)
            {
                await SetUnauthorizedResponse(context);
                return;
            }

            bool tokenIsValid = await ValidateToken(token);

            if (!tokenIsValid)
            {
                await SetUnauthorizedResponse(context);
                return;
            }

            await AttachUserToContext(context, token);
            await next(context);
        }

        private async Task<bool> HasAllowAnonymousAtrribute(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint is not null)
            {
                var allowAnonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
                return allowAnonymous != null;
            }

            return false;
        }

        private async Task SetUnauthorizedResponse(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }

        private async Task<bool> ValidateToken(string token)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            string bodyData = JsonSerializer.Serialize(new { Token = token });
            StringContent data = new StringContent(bodyData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(appSettings.ValidateUrl, data);
            bool isValid = false;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                isValid = JsonSerializer.Deserialize<bool>(result);
            }

            return isValid;
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                string responseUserData = await client.GetStringAsync($"{appSettings.DataUrl}?token={token}");
                UserDataModel user = JsonSerializer.Deserialize<UserDataModel>(responseUserData);
                SignIn(context, user.userName, user.userId);
            }
            catch (Exception)
            {
            }
        }

        private void SignIn(HttpContext context, string login, int userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, login)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            context.User = new ClaimsPrincipal(identity);
        }
    }
}
