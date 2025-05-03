using System.Security.Claims;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse
                {
                    Succeeded = false,
                    Message = "Registration failed.",
                    Errors = result.Errors
                        .Select(e => e.Description)
                        .ToList()
                });
            }

            // TODO: Add to "User" role

            _logger.LogInformation("User {UserId} ({Email}) registered successfully", user.Id, user.Email);
            return CreatedAtAction("GetUserById", new { id = user.Id }, new ApiResponse
            {
                Succeeded = true,
                Message = "Registration successful.",
                Data = new { userId = user.Id }
            });
        }

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

        [Authorize]
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