using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ManagerWeb.Models;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IUserService userService;
        private readonly HashManager hashManager;

        public UserController(IUserService userService)
        {
            this.userService = userService;
            this.hashManager = new HashManager();
        }

        [AllowAnonymous]
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            return this.View(new UserModel());
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromForm] UserModel model)
        {
            string passwordHash = this.hashManager.HashPasswordToSha512(model.Password);
            UserModel user = await userService.Authenticate(model.Login, passwordHash).ConfigureAwait(false);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return this.View(user);
        }
    }
}