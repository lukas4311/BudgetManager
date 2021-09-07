using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BudgetManager.Api.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BudgetManager.Api.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AuthApiSetting appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthApiSetting> appSettings)
        {
            this.next = next;
            this.appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await attachUserToContext(context, token);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }

            await next(context);
        }

        private async Task attachUserToContext(HttpContext context, string token)
        {
            try
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

                if (isValid)
                {
                    string responseUserData = await client.GetStringAsync($"{this.appSettings.DataUrl}?token={token}");
                    UserDataModel userData = JsonSerializer.Deserialize<UserDataModel>(responseUserData);
                    context.Items["User"] = userData;
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
