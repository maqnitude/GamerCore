using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.Api.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockService.Object, _mockLogger.Object);
        }

        #region GetProductsAsync tests

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WhenProductsExist()
        {
            // Arrange
            var productDtos = new List<ProductDto>
            {
                new() { ProductId = 1, Name = "Product 1" },
                new() { ProductId = 2, Name = "Product 2" }
            };

            var pagedResult = new PaginatedList<ProductDto>
            {
                Items = productDtos,
                Page = 1,
                PageSize = 10,
                TotalItems = 2
            };

            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<int[]?>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPagedResult = Assert.IsAssignableFrom<PaginatedList<ProductDto>>(okResult.Value);
            Assert.Equal(pagedResult, returnedPagedResult);

            _mockService.Verify(s => s.GetFilteredProductsAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<int[]?>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ParsesCategoryIds_Correctly()
        {
            // Arrange
            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<int[]?>()))
                .ReturnsAsync(new PaginatedList<ProductDto>());

            // Act
            await _controller.GetProductsAsync(categoryIds: "1,2,3");

            // Assert
            _mockService.Verify(s => s.GetFilteredProductsAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.Is<int[]>(arr => arr.SequenceEqual(new[] { 1, 2, 3 }))),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsNoContent_WhenNoProductsExist()
        {
            // Arrange
            var emptyPagedResult = new PaginatedList<ProductDto>
            {
                Items = new List<ProductDto>(),
                Page = 1,
                PageSize = 10,
                TotalItems = 0
            };

            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<int[]?>()))
                .ReturnsAsync(emptyPagedResult);

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<int[]?>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal Server Error", objectResult.Value);

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

        #region GetProductDetailsAsync tests

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsOk_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var productDetailsDto = new ProductDetailsDto { ProductId = productId, Name = "Test Product" };

            _mockService.Setup(service => service.GetProductDetailsAsync(productId))
                .ReturnsAsync(productDetailsDto);

            // Act
            var result = await _controller.GetProductDetailsAsync(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProductDetails = Assert.IsType<ProductDetailsDto>(okResult.Value);
            Assert.Equal(productDetailsDto, returnedProductDetails);
        }

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentProductId = 999;

            _mockService.Setup(service => service.GetProductDetailsAsync(nonExistentProductId))
                .ReturnsAsync((ProductDetailsDto?)null);

            // Act
            var result = await _controller.GetProductDetailsAsync(nonExistentProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int productId = 1;

            _mockService.Setup(service => service.GetProductDetailsAsync(productId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetProductDetailsAsync(productId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal Server Error", objectResult.Value);

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