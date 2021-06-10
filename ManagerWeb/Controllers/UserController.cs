using System.Threading.Tasks;
using BudgetManager.ManagerWeb.Models;
using BudgetManager.ManagerWeb.Resources;
using BudgetManager.ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.ManagerWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IUserService userService;
        private readonly IHashManager hashManager;

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

            if (user is null)
            {
                this.ModelState.AddModelError(nameof(Resource.InvalidUserOrPassword), Resource.InvalidUserOrPassword);
                return this.View(user);
            }

            int userId = this.userService.GetUserId(user.Login);
            await this.userService.SignIn(user.Login, userId).ConfigureAwait(false);
            return this.RedirectToAction("Index", "Home");
        }
    }
}