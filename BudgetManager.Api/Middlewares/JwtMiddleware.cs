using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BudgetManager.Api.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
                await attachUserToContext(context, userService, token);

            await next(context);
        }

        private async Task attachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                //TODO: send request to auth API
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(this.appSettings.Url);
                bool isValid = JsonSerializer.Deserialize<bool>(response);
                //var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                //// attach user to context on successful jwt validation
                //context.Items["User"] = userService.GetUserId(userId);
            }
            catch
            {
            }
        }
    }
}
