using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BudgetManager.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BudgetManager.Api.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AuthApiSetting appSettings;
        private readonly HttpClient httpClient;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthApiSetting> appSettings)
        {
            this.next = next;
            this.appSettings = appSettings.Value;
            this.httpClient = new HttpClient();
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                await this.SetUnauthorizedResponse(context);
                return;
            }

            bool tokenIsValid = await this.ValidateToken(token);

            if (!tokenIsValid)
            {
                await this.SetUnauthorizedResponse(context);
                return;
            }

            await this.AttachUserToContext(context, token);
            await next(context);
        }

        private async Task SetUnauthorizedResponse(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }

        private async Task<bool> ValidateToken(string token)
        {
            HttpClient client = new HttpClient();
            string bodyData = JsonSerializer.Serialize(new { Token = token });
            StringContent data = new StringContent(bodyData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(this.appSettings.ValidateUrl, data);
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
                HttpClient client = new HttpClient();
                //string bodyData = JsonSerializer.Serialize(new { Token = token });
                //StringContent data = new StringContent(bodyData, Encoding.UTF8, "application/json");
                //HttpResponseMessage response = await client.PostAsync(this.appSettings.ValidateUrl, data);
                //bool isValid = false;

                //if (response.IsSuccessStatusCode)
                //{
                //    string result = response.Content.ReadAsStringAsync().Result;
                //    isValid = JsonSerializer.Deserialize<bool>(result);
                //}

                //if (!isValid)
                //    return;

                string responseUserData = await client.GetStringAsync($"{this.appSettings.DataUrl}?token={token}");
                UserDataModel user = JsonSerializer.Deserialize<UserDataModel>(responseUserData);
                this.SignIn(context, user.userName, user.userId);
            }
            catch (Exception e)
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
