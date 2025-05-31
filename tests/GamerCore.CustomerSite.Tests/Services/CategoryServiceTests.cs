using System.Net;
using System.Text;
using System.Text.Json;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace GamerCore.CustomerSite.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<CategoryService>> _mockLogger;
        private readonly CategoryService _service;

        // Simulate HTTP responses without making actual network calls
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public CategoryServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<CategoryService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Set up the HTTP client factory to return our mocked client
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com")
            };

            _mockHttpClientFactory.Setup(factory => factory.CreateClient("GamerCoreDev"))
                .Returns(httpClient);

            _service = new CategoryService(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        #region GetCategoriesAsync tests

        [Fact]
        public async Task GetCategoriesAsync_ReturnsCategories_WhenApiCallSucceeds()
        {
            // Arrange
            var categories = new List<CategoryViewModel>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Category 1"},
                new() { Id = Guid.NewGuid().ToString(), Name = "Category 2"},
            };

            var jsonResponse = JsonSerializer.Serialize(categories);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/categories")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetCategoriesAsync();

            // Assert
            Assert.Equal(categories.Count, result.Count);

            for (int i = 0; i < categories.Count; i++)
            {
                Assert.Equal(categories[i].Id, result[i].Id);
                Assert.Equal(categories[i].Name, result[i].Name);
            }
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsEmptyList_WhenDeserializationReturnsNull()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetCategoriesAsync();

            // Assert
            Assert.Empty(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCategoriesAsync_ThrowsHttpRequestException_WhenApiCallFails()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(_service.GetCategoriesAsync);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCategoriesAsync_ThrowsJsonException_WhenDeserializationFails()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("not valid json", Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(_service.GetCategoriesAsync);

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
    }
}