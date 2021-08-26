using BudgetManager.AuthApi.Models;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BudgetManager.AuthApi.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtService jwtService;

        public AuthController(IUserService userService, IJwtService jwtService, IOptions<JwtSettingOption> options)
        {
            this.userService = userService;
            this.jwtService = jwtService;
            this.jwtService.SetUp(new JwtSetting(options.Value.Secret, options.Value.Expiration));
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserModel model)
        {
            UserIdentification userInfo = this.userService.Authenticate(model.UserName, model.Password);

            if(userInfo is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            string token = this.jwtService.GenerateToken(userInfo);
            return Ok(token);
        }
    }
}
