using System.Security.Claims;
using GamerCore.Api.Services;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            UserManager<User> userManager,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<object>.CreateError(
                        StatusCodes.Status400BadRequest,
                        result.ErrorMessage!));
                }

                var user = result.User!;

                _logger.LogInformation("User {UserId} ({Email}) registered successfully", user.Id, user.Email);

                return CreatedAtAction(
                    "GetUserById",
                    "Users",
                    new { id = user.Id },
                    ApiResponse<UserDto>.CreateSuccess(new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    })
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registerting.");
                return StatusCode(500, ApiResponse<object>.CreateError(
                    StatusCodes.Status500InternalServerError,
                    "Internal server error."));
            }
        }

        /// <summary>
        /// This is used for customer login.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (!result.Succeeded)
                {
                    switch (result.ResultType)
                    {
                        case LoginResultType.LockedOut:
                            return BadRequest(ApiResponse<object>.CreateError(
                                StatusCodes.Status400BadRequest,
                                result.ErrorMessage!));

                        default:
                            return Unauthorized(ApiResponse<object>.CreateError(
                                StatusCodes.Status401Unauthorized,
                                result.ErrorMessage ?? "Invalid email or password."));
                    }
                }

                var user = result.User!;
                // IMPORTANT: Need role for role based authorization
                var userRoles = await _userManager.GetRolesAsync(user);

                return Ok(ApiResponse<UserDto>.CreateSuccess(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = userRoles.ToList()
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                return StatusCode(500, ApiResponse<object>.CreateError(
                    StatusCodes.Status500InternalServerError,
                    "Internal server error."));
            }
        }

        /// <summary>
        /// This is used for admin login.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginJwt([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

            try
            {
                var result = await _authService.LoginJwtAsync(loginDto);

                if (!result.Succeeded)
                {
                    switch (result.ResultType)
                    {
                        case LoginResultType.LockedOut:
                            return BadRequest(ApiResponse<object>.CreateError(
                                StatusCodes.Status400BadRequest,
                                result.ErrorMessage!));

                        case LoginResultType.AccessDenied:
                            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.CreateError(
                                StatusCodes.Status403Forbidden,
                                result.ErrorMessage!));

                        default:
                            return Unauthorized(ApiResponse<object>.CreateError(
                                StatusCodes.Status401Unauthorized,
                                result.ErrorMessage ?? "Invalid email or password."));
                    }
                }

                return Ok(ApiResponse<string>.CreateSuccess(result.AccessToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                return StatusCode(500, ApiResponse<object>.CreateError(
                    StatusCodes.Status500InternalServerError,
                    "Internal server error."));
            }
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _authService.LogoutAsync(User);

                if (!result)
                {
                    return BadRequest(ApiResponse<object>.CreateError(
                        StatusCodes.Status400BadRequest,
                        "No active user session found."));
                }

                return Ok(ApiResponse<object>.CreateSuccess(null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging out.");
                return StatusCode(500, ApiResponse<object>.CreateError(
                    StatusCodes.Status500InternalServerError,
                    "Internal server error."));
            }
        }

        // For JWT auth, client simply discards the token
        // No server side action needed, but we can provide an endpoint for logging
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("admin/logout")]
        public IActionResult LogoutJwt()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            _logger.LogInformation("User {UserId} ({Email}) logged out sucessfully.", userId, email);

            return Ok(ApiResponse<object>.CreateSuccess(null));
        }
    }
}