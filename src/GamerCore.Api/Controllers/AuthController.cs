using System.Security.Claims;
using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

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
                ApiResponse<AppUserDto>.CreateSuccess(new AppUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                })
            );
        }

        /// <summary>
        /// This is used for customer login.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

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

            return Ok(ApiResponse<AppUserDto>.CreateSuccess(new AppUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName
            }));
        }

        /// <summary>
        /// This is used for admin login.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("Admin/Login")]
        public async Task<IActionResult> LoginJwtAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.CreateError(
                    StatusCodes.Status400BadRequest,
                    "Invalid request data."));
            }

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

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
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

        // For JWT auth, client simply discards the token
        // No server side action needed, but we can provide an endpoint for logging
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Admin/Logout")]
        public IActionResult LogoutJwt()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            _logger.LogInformation("User {UserId} ({Email}) logged out sucessfully.", userId, email);

            return Ok(ApiResponse<object>.CreateSuccess(null));
        }
    }
}