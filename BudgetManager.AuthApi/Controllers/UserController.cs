using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.AuthApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("user/v{version:apiVersion}")]
public class UserController: ControllerBase
{
    private const string UserDefinitionIsRequired = "User definition is not valid";
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Endpoint to register new user
    /// </summary>
    /// <param name="userCreateModel">User model</param>
    /// <returns>Action result</returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("register"), MapToApiVersion("1.0")]
    public IActionResult Register([FromBody] UserCreateModel userCreateModel)
    {
        if(userCreateModel is null)
            return BadRequest(new { message = UserDefinitionIsRequired });

        _userService.CreateUser(userCreateModel);
        return Ok();
    }
}