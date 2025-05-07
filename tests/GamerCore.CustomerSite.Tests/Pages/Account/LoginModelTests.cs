using System.Net;
using System.Text.Json;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Tests.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.CustomerSite.Pages.Account
{
    public class LoginModelTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<LoginModel>> _mockLogger;
        private readonly LoginModel _model;

        public LoginModelTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<LoginModel>>();
            _model = new LoginModel(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        #region OnGet tests

        [Fact]
        public void OnGet_SetsReturnUrl()
        {
            // Arrange
            var returnUrl = "/test";
            SetupPageContext(_model);

            // Act
            _model.OnGet(returnUrl);

            // Assert
            Assert.Equal(returnUrl, _model.ReturnUrl);
        }

        [Fact]
        public void OnGet_SetsDefaultReturnUrl_WhenReturnUrlIsNull()
        {
            // Arrange
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            // Act
            _model.OnGet(null);

            // Assert
            Assert.Equal("/", _model.ReturnUrl);
        }

        [Fact]
        public void OnGet_AddsModelError_WhenErrorMessageHasValue()
        {
            // Arrange
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            _model.ErrorMessage = "Login failed";

            // Act
            _model.OnGet(null);

            // Assert
            Assert.True(_model.ModelState.Count > 0);
            Assert.Equal("Login failed", _model.ModelState[string.Empty]?.Errors[0].ErrorMessage);
        }

        #endregion

        #region OnPostAsync tests

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
        {
            // Arrange
            _model.ModelState.AddModelError("Email", "Required");
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_RedirectsToReturnUrl_WhenLoginSucceeds()
        {
            // Arrange
            var userDto = new AppUserDto
            {
                Id = "123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com"
            };

            var apiResponse = ApiResponse<AppUserDto>.CreateSuccess(userDto);
            var json = JsonSerializer.Serialize(apiResponse);
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, json);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            _model.Input = new LoginModel.InputModel
            {
                Email = "john@example.com",
                Password = "Password1!",
                RememberMe = false
            };
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            var returnUrl = "/home";

            // Act
            var result = await _model.OnPostAsync(returnUrl);

            // Assert
            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }

        [Fact]
        public async Task OnPostAsync_AddsErrorAndReturnsPage_WhenLoginFails()
        {
            // Arrange
            var handler = new MockHttpMessageHandler(HttpStatusCode.BadRequest);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            _model.Input = new LoginModel.InputModel
            {
                Email = "john@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.True(_model.ModelState.ErrorCount > 0);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenExceptionIsThrown()
        {
            // Arrange
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev"))
                .Throws(new Exception("Test Exception"));

            _model.Input = new LoginModel.InputModel
            {
                Email = "john@example.com",
                Password = "Password1!",
                RememberMe = false
            };
            SetupPageContext(_model);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Content("~/")).Returns("/");
            _model.Url = mockUrlHelper.Object;

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.True(_model.ModelState.ErrorCount > 0);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        private void SetupPageContext(LoginModel model)
        {
            // Create a minimal cookie auth service so SignInAsync doesn't throw exception
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
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            model.TempData = tempData;
        }
    }
}