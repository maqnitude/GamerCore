using System.Security.Claims;
using GamerCore.Api.Services;
using GamerCore.Core.Constants;
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
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = true,
                    Message = "Invalid request data.",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Registration failed.",
                    Errors = ["Email already in use"]
                });
            }

            // Use email for username to simplify things because
            // emails are inherently unique
            var user = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                Email = registerDto.Email
                // TODO: Add security stamp
            };

            var userResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!userResult.Succeeded)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Registration failed.",
                    Errors = userResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Registration failed.",
                    Errors = roleResult.Errors
                        .Select(e => e.Description)
                        .ToList()
                });
            }

            _logger.LogInformation("User {UserId} ({Email}) registered successfully", user.Id, user.Email);
            return CreatedAtAction("GetUserById", new { id = user.Id }, new ApiResponse
            {
                Succeeded = true,
                Message = "Registration successful.",
                Data = new { userId = user.Id }
            });
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
                return BadRequest(new ApiResponse
                {
                    Succeeded = true,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Invalid email or password."
                });
            }

            // TODO: Check email confirmation

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} ({Email}) logged in successfully", user.Id, user.Email);
                return Ok(new ApiResponse
                {
                    Succeeded = true,
                    Message = "Login successful",
                    Data = new AppUserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    }
                });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} ({Email}) is locked out", user.Id, user.Email);
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Account locked. Try again later."
                });
            }

            return Unauthorized(new ApiResponse
            {
                Succeeded = false,
                Message = "Invalid email or password."
            });
        }

        /// <summary>
        /// This is used for admin login.
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("LoginJwt")]
        public async Task<IActionResult> LoginJwtAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = true,
                    Message = "Invalid request data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Invalid email or password."
                });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(RoleNames.Administrator))
                {
                    return StatusCode(403, new ApiResponse
                    {
                        Succeeded = false,
                        Message = "Access denied"
                    });
                }

                var token = await _jwtService.GenerateJwtTokenAsync(user);

                _logger.LogInformation("User {UserId} ({Email}) logged in with JWT successfully", user.Id, user.Email);

                return Ok(new ApiResponse
                {
                    Succeeded = true,
                    Message = "Login successful",
                    Data = new // LoginResponse
                    {
                        Token = token,
                        User = new AppUserDto
                        {
                            Id = user.Id,
                            Email = user.Email!,
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        }
                    }
                });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} ({Email}) is locked out", user.Id, user.Email);
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Account locked. Try again later."
                });
            }

            return Unauthorized(new ApiResponse
            {
                Succeeded = false,
                Message = "Invalid email or password."
            });
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User {UserId} ({Email}) logged out", userId, email);

            return Ok(new ApiResponse
            {
                Succeeded = true,
                Message = "Logged out successfully."
            });
        }

        // For JWT auth, client simply discards the token
        // No server side action needed, but we can provide an endpoint for logging
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("LogoutJwt")]
        public IActionResult LogoutJwt()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            _logger.LogInformation("User {UserId} ({Email}) logged out", userId, email);

            return Ok(new ApiResponse
            {
                Succeeded = true,
                Message = "Logged out successfully."
            });
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Succeeded = false,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse
            {
                Succeeded = true,
                Data = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName
                }
            });
        }
    }
}