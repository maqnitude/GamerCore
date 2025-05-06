using System.Security.Claims;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;

namespace GamerCore.Api.Services
{
    public interface IAuthService
    {
        Task<RegistrationResult> RegisterAsync(RegisterDto registerDto);
        Task<LoginResult> LoginAsync(LoginDto loginDto);
        Task<LoginResult> LoginJwtAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(ClaimsPrincipal user);
    }

    public class RegistrationResult
    {
        public bool Succeeded { get; private set; }
        public string? ErrorMessage { get; private set; }
        public AppUser? User { get; private set; }

        private RegistrationResult(bool succeeded, string? errorMessage = null, AppUser? user = null)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
            User = user;
        }

        public static RegistrationResult Success(AppUser user)
        {
            return new RegistrationResult(true, null, user);
        }

        public static RegistrationResult Error(string errorMessage)
        {
            return new RegistrationResult(false, errorMessage);
        }
    }

    public enum LoginResultType
    {
        Success,
        InvalidCredentials,
        LockedOut,
        AccessDenied
    }

    /// <summary>
    /// This is used for both customer and admin login.
    /// </summary>
    public class LoginResult
    {
        public bool Succeeded { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? AccessToken { get; private set; }
        public AppUser? User { get; private set; }

        public LoginResultType ResultType { get; private set; }

        private LoginResult(
            LoginResultType resultType,
            string? errorMessage = null,
            string? accessToken = null,
            AppUser? user = null)
        {
            ResultType = resultType;
            Succeeded = resultType == LoginResultType.Success;
            ErrorMessage = errorMessage;
            AccessToken = accessToken;
            User = user;
        }

        public static LoginResult Success(string? accessToken = null, AppUser? user = null)
        {
            return new LoginResult(LoginResultType.Success, null, accessToken, user);
        }

        public static LoginResult InvalidCredentials()
        {
            return new LoginResult(LoginResultType.InvalidCredentials, "Invalid email or password.");
        }

        public static LoginResult LockedOut()
        {
            return new LoginResult(LoginResultType.LockedOut, "Account locked. Try again later.");
        }

        public static LoginResult AccessDenied()
        {
            return new LoginResult(LoginResultType.AccessDenied, "Access denied.");
        }
    }
}