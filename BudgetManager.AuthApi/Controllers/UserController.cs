using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.AuthApi.Controllers;

[Route("user")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("register")]
    public IActionResult Validate([FromBody] UserCreateModel userCreateModel)
    {
        if(userCreateModel is null)
            return BadRequest(new { message = "User definition is required" });

        _userService.CreateUser(userCreateModel);
        return Ok();
    }
}