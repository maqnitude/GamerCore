using System.Security.Claims;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace GamerCore.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return RegistrationResult.Error("Email already in use.");
            }

            // Use email for username to simplify things because
            // emails are inherently unique
            var user = new User
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
                return RegistrationResult.Error("Registration failed.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, RoleConstants.Customer);
            if (!roleResult.Succeeded)
            {
                // Delete the created user too
                await _userManager.DeleteAsync(user);

                return RegistrationResult.Error("Registration failed.");
            }

            return RegistrationResult.Success(user);
        }

        /// <summary>
        /// [DEPRECATED]
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        public async Task<LoginResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return LoginResult.InvalidCredentials();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} ({Email}) logged in successfully", user.Id, user.Email);

                return LoginResult.Success(null, user);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} ({Email}) is locked out", user.Id, user.Email);

                return LoginResult.LockedOut();
            }

            return LoginResult.InvalidCredentials();
        }

        /// <summary>
        /// [DEPRECATED]
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        public async Task<LoginResult> LoginJwtAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return LoginResult.InvalidCredentials();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? loginDto.Email,
                loginDto.Password,
                loginDto.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(RoleConstants.Admin))
                {
                    return LoginResult.AccessDenied();
                }

                var accessToken = await _jwtService.GenerateJwtTokenAsync(user);

                _logger.LogInformation("User {UserId} ({Email}) logged in with JWT successfully", user.Id, user.Email);

                return LoginResult.Success(accessToken);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} ({Email}) is locked out", user.Id, user.Email);

                return LoginResult.LockedOut();
            }

            return LoginResult.InvalidCredentials();
        }

        /// <summary>
        /// [DEPRECATED]
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> LogoutAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = user.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return false;
            }

            await _signInManager.SignOutAsync();

            return true;
        }
    }
}