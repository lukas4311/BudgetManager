using BudgetManager.AuthApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.AuthApi.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserModel model)
        {
            // TODO: 
            // 1. call user service to verify user name and password
            // 2. if true -> generate token, return generated token 
            // 1. if false return 400

            return BadRequest(new { message = "Username or password is incorrect" });
        }
    }
}
