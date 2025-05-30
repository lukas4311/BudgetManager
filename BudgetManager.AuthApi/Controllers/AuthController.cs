﻿using Asp.Versioning;
using BudgetManager.AuthApi.Models;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace BudgetManager.AuthApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("auth/v{version:apiVersion}")]
    [Produces("application/json", "application/problem+json")]
    public class AuthController : ControllerBase
    {
        private const string TokenIsRequired = "Token is required";
        private const string UsernameOrPasswordIsIncorrect = "Username or password is incorrect";
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService, IOptions<JwtSettingOption> options)
        {
            _userService = userService;
            _jwtService = jwtService;
            _jwtService.SetUp(new JwtSetting(options.Value.Secret, options.Value.Expiration));
        }

        /// <summary>
        /// Method to authenticate user and get token
        /// </summary>
        /// <param name="model">Model containing authentication model</param>
        /// <returns>Model containing token and user info</returns>
        [HttpPost("authenticate"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(AuthResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<AuthResponseModel> Authenticate([FromBody] UserModel model)
        {
            UserIdentification userInfo = _userService.Authenticate(model.UserName, model.Password);

            if (userInfo is null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = UsernameOrPasswordIsIncorrect,
                    Status = StatusCodes.Status400BadRequest
                });

            string token = _jwtService.GenerateToken(userInfo);
            Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            return Ok(new AuthResponseModel(token, userInfo.UserId, userInfo.UserName));
        }

        /// <summary>
        /// Method to authenticate user and get token
        /// </summary>
        /// <param name="model">Model containing authentication model</param>
        /// <returns>Model containing token and user info</returns>
        [HttpPost("authenticate"), MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(AuthResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<AuthResponseModel> AuthenticateV2([FromBody] UserModel model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to validate token
        /// </summary>
        /// <param name="tokenModel">Token model</param>
        /// <returns><see langword="true"/> if token is valid</returns>
        [HttpPost("validate"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> Validate([FromBody] TokenModel tokenModel)
        {
            if (tokenModel is null || string.IsNullOrEmpty(tokenModel.Token))
                return BadRequest(new ProblemDetails
                {
                    Title = TokenIsRequired,
                    Detail = TokenIsRequired,
                    Status = StatusCodes.Status400BadRequest
                });

            bool isValid = _jwtService.IsTokenValid(tokenModel.Token);
            return Ok(isValid);
        }

        /// <summary>
        /// Method to get user data using access token
        /// </summary>
        /// <param name="token">Model for access token</param>
        /// <returns>Model with user data</returns>
        [HttpGet("tokenData"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(UserIdentification), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult<UserIdentification> GetTokenData([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new ProblemDetails
                {
                    Title = TokenIsRequired,
                    Detail = TokenIsRequired,
                    Status = StatusCodes.Status400BadRequest
                });

            UserIdentification userIdentification = _jwtService.GetUserIdentification(token);
            return Ok(userIdentification);
        }
    }
}