using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Tests.Utilities;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GamerCore.Api.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithDefaultPagination_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            var productDtos = pagedResult.Items;
            Assert.Equal(products.Count, productDtos.Count);

            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(products[i].ProductId, productDtos[i].ProductId);
                Assert.Equal(products[i].Name, productDtos[i].Name);
                Assert.Equal(products[i].Price, productDtos[i].Price);
                Assert.Equal(products[i].ProductCategories.First().Category.Name, productDtos[i].Categories.First().Name);
            }

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithCustomPagination_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 10, categoryCount: 2);

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetProductsAsync(page: 2, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            var productDtos = pagedResult.Items;

            Assert.Equal(5, productDtos.Count);
            Assert.Equal(6, productDtos[0].ProductId);
            Assert.Equal(10, productDtos[^1].ProductId);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithOutOfRangePageNumber()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 10, categoryCount: 2);

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var resultLo = await _controller.GetProductsAsync(page: -1, pageSize: 5);
            // var resultHi = await controller.GetProductsAsync(page: 999, pageSize: 5);

            // Assert
            var okResultLo = Assert.IsType<OkObjectResult>(resultLo.Result);
            // var okResultHi = Assert.IsType<OkObjectResult>(resultHi.Result);
            var pagedResultLo = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultLo.Value);
            // var pagedResultHi = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultHi.Value);
            Assert.Equal(1, pagedResultLo.Page);
            // Assert.Equal(4, pagedResultHi.Page);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithOutOfRangePageSize()
        {
            // Arrange
            var products = ProductGenerator.Generate();

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var resultLo = await _controller.GetProductsAsync(page: 1, pageSize: 0);
            var resultHi = await _controller.GetProductsAsync(page: 1, pageSize: 999);

            // Assert
            var okResultLo = Assert.IsType<OkObjectResult>(resultLo.Result);
            var okResultHi = Assert.IsType<OkObjectResult>(resultHi.Result);
            var pagedResultLo = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultLo.Value);
            var pagedResultHi = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultHi.Value);
            Assert.Equal(1, pagedResultLo.PageSize);
            Assert.Equal(PaginationConstants.MaxPageSize, pagedResultHi.PageSize);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithFilteringByCategory_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 10);

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetProductsAsync(categoryIds: "1,2,3");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            Assert.Equal(15, pagedResult.TotalItems);
            Assert.Equal(ProductGenerator.Categories[0].Name, pagedResult.Items[0].Categories.ElementAt(0).Name);
            Assert.Equal(ProductGenerator.Categories[1].Name, pagedResult.Items[5].Categories.ElementAt(0).Name);
            Assert.Equal(ProductGenerator.Categories[2].Name, pagedResult.Items[10].Categories.ElementAt(0).Name);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithPaginationAndFilteringByCategory_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 20, categoryCount: 10);

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetProductsAsync(page: 2, pageSize: 20, categoryIds: "1,2,3");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            Assert.Equal(ProductGenerator.Categories[1].Name, pagedResult.Items[0].Categories.ElementAt(0).Name);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsNoContent_WhenNoProductExists()
        {
            // Arrange
            var products = Enumerable.Empty<Product>();

            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetQueryableProducts())
                .Throws(new Exception("Test Exception"));

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
    }
}