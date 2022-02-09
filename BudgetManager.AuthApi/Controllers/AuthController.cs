using System.Linq;
using BudgetManager.AuthApi.Models;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
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
        public ActionResult<AuthResponseModel> Authenticate([FromBody] UserModel model)
        {
            UserIdentification userInfo = this.userService.Authenticate(model.UserName, model.Password);

            if(userInfo is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            string token = this.jwtService.GenerateToken(userInfo);
            Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            return Ok(new AuthResponseModel(token, userInfo.UserId, userInfo.UserName));
        }

        [HttpPost("validate")]
        public ActionResult<bool> Validate([FromBody] TokenModel tokenModel)
        {
            if(tokenModel is null || string.IsNullOrEmpty(tokenModel.Token))
                return BadRequest(new { message = "Token is required" });

            bool isValid = this.jwtService.IsTokenValid(tokenModel.Token);
            return Ok(isValid);
        }

        [HttpGet("tokenData")]
        public ActionResult<UserIdentification> GetTokenData([FromQuery]string token)
        {
            if(string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            UserIdentification userIdentification = this.jwtService.GetUserIdentification(token);
            return Ok(userIdentification);
        }
    }
}
