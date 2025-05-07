using System.Net;
using System.Text;
using System.Text.Json;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace GamerCore.CustomerSite.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        // Simulate HTTP responses without making actual network calls
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public ProductServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Set up the HTTP client factory to return our mocked client
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com")
            };

            _mockHttpClientFactory.Setup(factory => factory.CreateClient("GamerCoreDev"))
                .Returns(httpClient);

            _service = new ProductService(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        #region GetProductsAsync tests

        [Fact]
        public async Task GetProductsAsync_ReturnsProducts_WhenApiCallSucceeds()
        {
            // Arrange
            var paginatedList = new PaginatedList<ProductViewModel>
            {
                Items = new List<ProductViewModel>
                {
                    new() { ProductId = 1, Name = "Product 1", Price = 99.99m },
                    new() { ProductId = 2, Name = "Product 2", Price = 199.99m }
                },
                Page = 1,
                PageSize = 10,
                TotalItems = 2
            };

            var jsonResponse = JsonSerializer.Serialize(paginatedList);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products?page=1")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetProductsAsync(page: 1);

            // Assert
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1, result.Items[0].ProductId);
            Assert.Equal("Product 1", result.Items[0].Name);
            Assert.Equal(99.99m, result.Items[0].Price);
            Assert.Equal(2, result.Items[1].ProductId);
            Assert.Equal("Product 2", result.Items[1].Name);
            Assert.Equal(199.99m, result.Items[1].Price);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalItems);
        }

        [Fact]
        public async Task GetProductsAsync_ThrowsHttpRequestException_WhenApiCallFails()
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
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetProductsAsync());

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsEmptyResult_WhenDeserializationReturnsNull()
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
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetProductsAsync_ThrowsJsonException_WhenDeserializationFails()
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
            await Assert.ThrowsAsync<JsonException>(() => _service.GetProductsAsync());

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

        #region GetFeaturedProductsAsync tests

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsProducts_WhenApiCallSucceeds()
        {
            // Arrange
            var featuredProducts = new List<ProductViewModel>
            {
                new() { ProductId = 1, Name = "Featured Product 1", Price = 99.99m, IsFeatured = true },
                new() { ProductId = 2, Name = "Featured Product 2", Price = 199.99m, IsFeatured = true }
            };

            var jsonResponse = JsonSerializer.Serialize(featuredProducts);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products/Featured")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetFeaturedProductsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].ProductId);
            Assert.Equal("Featured Product 1", result[0].Name);
            Assert.Equal(99.99m, result[0].Price);
            Assert.Equal(2, result[1].ProductId);
            Assert.Equal("Featured Product 2", result[1].Name);
            Assert.Equal(199.99m, result[1].Price);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ThrowsHttpRequestException_WhenApiCallFails()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products/Featured")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetFeaturedProductsAsync());

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsEmptyList_WhenDeserializationReturnsNull()
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
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products/Featured")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetFeaturedProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ThrowsJsonException_WhenDeserializationFails()
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
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products/Featured")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _service.GetFeaturedProductsAsync());

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

        #region GetProductDetailsAsync tests

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsProductDetails_WhenApiCallSucceeds()
        {
            // Arrange
            var productDetails = new ProductDetailsViewModel
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 99.99m,
                DescriptionHtml = "<p>Test description</p>",
                WarrantyHtml = "<p>Test warranty</p>"
            };

            var jsonResponse = JsonSerializer.Serialize(productDetails);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "applications/json")
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                        && req.RequestUri!.ToString().Contains("/api/Products/1")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetProductDetailsAsync(1);

            // Assert
            Assert.Equal(1, result.ProductId);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal(99.99m, result.Price);
            Assert.Equal("<p>Test description</p>", result.DescriptionHtml);
            Assert.Equal("<p>Test warranty</p>", result.WarrantyHtml);
        }

        [Fact]
        public async Task GetProductDetailsAsync_ThrowsHttpRequestException_WhenApiCallFails()
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
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetProductDetailsAsync(1));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        #endregion
    }
}