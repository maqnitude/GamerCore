using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamerCore.CustomerSite.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly string _apiBaseEndpoint = "/api/Auth";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LoginModel> _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            string apiEndpoint = _apiBaseEndpoint + "/Login";

            try
            {
                var loginDto = new LoginDto
                {
                    Email = Input.Email,
                    Password = Input.Password,
                    RememberMe = Input.RememberMe
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(loginDto),
                    Encoding.UTF8,
                    "application/json");

                // First login via API then login locally via HttpContext
                var httpClient = _httpClientFactory.CreateClient("GamerCoreDev");
                using var response = await httpClient.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    using var streamContent = await response.Content.ReadAsStreamAsync();
                    var apiResponse = await JsonSerializer
                        .DeserializeAsync<ApiResponse<AppUserDto>>(streamContent, _jsonSerializerOptions);

                    if (apiResponse?.Data != null)
                    {
                        var user = apiResponse.Data;

                        // New claims for user
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, user!.Id),
                            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                            new(ClaimTypes.Email, user.Email)
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(Input.RememberMe ? 30 : 1)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            authProperties);

                        _logger.LogInformation("User {Email} logged in successfully", Input.Email);
                        return LocalRedirect(returnUrl);
                    }
                }

                // Handle failed login
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return Page();
            }
        }
    }
}