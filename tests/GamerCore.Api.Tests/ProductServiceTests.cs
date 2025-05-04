using GamerCore.Api.Services;
using GamerCore.Api.Tests.Utilities;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GamerCore.Api.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<ProductService>>();

            _mockUnitOfWork.SetupGet(uow => uow.Products).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.SetupGet(uow => uow.Categories).Returns(_mockCategoryRepository.Object);

            _service = new ProductService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        #region GetFilteredProductsAsync tests

        [Fact]
        public async Task GetFilteredProductsAsync_ReturnsProducts_WithDefaultPagination_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFilteredProductsAsync(1, null, null);

            // Assert
            Assert.Equal(products.Count, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(PaginationDefaults.PageSize, result.PageSize);
            Assert.Equal(products.Count, result.TotalItems);

            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(products[i].ProductId, result.Items[i].ProductId);
                Assert.Equal(products[i].Name, result.Items[i].Name);
                Assert.Equal(products[i].Price, result.Items[i].Price);
                Assert.Equal(products[i].ProductCategories.First().Category.Name,
                             result.Items[i].Categories.First().Name);
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
        public async Task GetFilteredProductsAsync_ReturnsProducts_WithCustomPagination_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 10, categoryCount: 2);

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFilteredProductsAsync(2, 5, null);

            // Assert
            Assert.Equal(5, result.Items.Count);
            Assert.Equal(2, result.Page);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(20, result.TotalItems);
            Assert.Equal(6, result.Items[0].ProductId);
            Assert.Equal(10, result.Items[^1].ProductId);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_ReturnsEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var products = Enumerable.Empty<Product>();

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFilteredProductsAsync(1, null, null);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        #endregion

        #region GetProductDetailsAsync tests

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsProductDetails_WhenProductExists()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);
            int productId = products.First().ProductId;

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetProductDetailsAsync(productId);

            // Assert
            Assert.NotNull(result);
            var expectedProduct = products.First();
            Assert.Equal(expectedProduct.ProductId, result.ProductId);
            Assert.Equal(expectedProduct.Name, result.Name);
            Assert.Equal(expectedProduct.Price, result.Price);
            Assert.Equal(expectedProduct.Detail.DescriptionHtml, result.DescriptionHtml);
            Assert.Equal(expectedProduct.Detail.WarrantyHtml, result.WarrantyHtml);
        }

        [Fact]
        public async Task GetProductDetailsAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentProductId = 999;
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetProductDetailsAsync(nonExistentProductId);

            // Assert
            Assert.Null(result);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        #endregion
    }
}