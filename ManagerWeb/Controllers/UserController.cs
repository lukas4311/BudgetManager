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
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            UserModel user = await _userService.Authenticate(model.Login, model.Password).ConfigureAwait(false);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return this.View(user);
        }
    }
}