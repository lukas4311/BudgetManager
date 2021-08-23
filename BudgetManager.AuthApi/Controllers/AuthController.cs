using BudgetManager.AuthApi.Models;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.AuthApi.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserModel model)
        {
            // TODO: 
            // 1. call user service to verify user name and password
            // 2. if true -> generate token, return generated token 
            // 1. if false return 400
            UserIdentification userInfo = this.userService.Authenticate(model.UserName, model.Password);

            if(userInfo is null)
                return BadRequest(new { message = "Username or password is incorrect" });

            
            return Ok();
        }
    }
}
