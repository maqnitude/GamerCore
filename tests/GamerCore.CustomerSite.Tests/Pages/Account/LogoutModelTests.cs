using System.Net;
using GamerCore.CustomerSite.Pages.Account;
using GamerCore.CustomerSite.Tests.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.CustomerSite.Tests.Pages.Account
{
    public class LogoutModelTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<LogoutModel>> _mockLogger;
        private readonly LogoutModel _model;

        public LogoutModelTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<LogoutModel>>();
            _model = new LogoutModel(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        #region OnPostAsync tests

        [Fact]
        public async Task OnPostAsync_RedirectsToReturnUrl_WhenLogoutSucceeds()
        {
            // Arrange
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            SetupPageContext(_model);

            var returnUrl = "/";

            // Act
            var result = await _model.OnPostAsync(returnUrl);

            // Assert
            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }

        [Fact]
        public async Task OnPostAsync_RedirectsToReturnUrl_WhenExceptionIsThrown()
        {
            // Arrange
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev"))
                .Returns(() => throw new HttpRequestException("Network error"));

            SetupPageContext(_model);

            var returnUrl = "/";

            // Act
            var result = await _model.OnPostAsync(returnUrl);

            // Assert
            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }

        [Fact]
        public async Task OnPostAsync_RedirectsToReturnUrl_LogoutFails()
        {
            // Arrange
            var handler = new MockHttpMessageHandler(HttpStatusCode.BadRequest);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            SetupPageContext(_model);

            var returnUrl = "/";

            // Act
            var result = await _model.OnPostAsync(returnUrl);

            // Assert
            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }

        #endregion

        private void SetupPageContext(LogoutModel model)
        {
            // Create a minimal cookie auth service so SignOutAsync doesn't throw exception
            var services = new ServiceCollection()
                .AddLogging()
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .Services;

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var pageContext = new PageContext(actionContext);
            model.PageContext = pageContext;
        }
    }
}