using System.Net;
using System.Text.Json;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Pages.Account;
using GamerCore.CustomerSite.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.CustomerSite.Tests.Pages.Account
{
    public class RegisterModelTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<RegisterModel>> _mockLogger;
        private readonly RegisterModel _model;

        public RegisterModelTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<RegisterModel>>();

            _model = new RegisterModel(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            // Arrange
            _model.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_RedirectsToConfirmation_WhenApiCallSucceeds()
        {
            // Arrange
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            _model.Input = new RegisterModel.InputModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("./RegisterConfirmation", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_AddsModelErrorAndReturnsPage_WhenApiCallFails()
        {
            // Arrange
            var apiResponse = ApiResponse<object>.CreateError(400, "Email in use.");
            var json = JsonSerializer.Serialize(apiResponse);
            var handler = new MockHttpMessageHandler(HttpStatusCode.BadRequest, json);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://test/") };
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev")).Returns(client);

            _model.Input = new RegisterModel.InputModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.True(_model.ModelState.ErrorCount > 0);
            Assert.Contains("Email in use", _model.ModelState[string.Empty]?.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenExceptionIsThrown()
        {
            // Arrange
            _mockHttpClientFactory.Setup(f => f.CreateClient("GamerCoreDev"))
                .Throws(new Exception("Network failure"));

            _model.Input = new RegisterModel.InputModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!"
            };

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
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}