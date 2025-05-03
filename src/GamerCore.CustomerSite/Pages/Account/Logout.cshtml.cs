using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamerCore.CustomerSite.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly string _apiBaseEndpoint = "/api/Auth";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(IHttpClientFactory httpClientFactory, ILogger<LogoutModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            string apiEndpoint = _apiBaseEndpoint + "/Logout";

            try
            {
                // Logout locally then logout via API
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var httpClient = _httpClientFactory.CreateClient("GamerCoreDev");

                try
                {
                    await httpClient.PostAsync(apiEndpoint, null);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error during API logout");
                }

                _logger.LogInformation("User logged out successfully");
                return LocalRedirect(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return LocalRedirect(returnUrl);
            }
        }
    }
}