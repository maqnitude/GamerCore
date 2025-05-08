using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Api.Tests.Utilities;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GamerCore.Api.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        private readonly Mock<IUserStore<AppUser>> _mockUserStore;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;

        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUserStore = new Mock<IUserStore<AppUser>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _mockUserManager = new Mock<UserManager<AppUser>>(_mockUserStore.Object,
                null, null, null, null, null, null, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            _mockLogger = new Mock<ILogger<ProductService>>();

            _mockUnitOfWork.SetupGet(uow => uow.Products).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.SetupGet(uow => uow.Categories).Returns(_mockCategoryRepository.Object);

            _service = new ProductService(_mockUnitOfWork.Object, _mockUserManager.Object, _mockLogger.Object);
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

        #region GetFeaturedProductsAsync tests

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsFeaturedProducts_WhenProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 10, categoryCount: 2);

            // Set some products as featured
            products[0].IsFeatured = true;
            products[2].IsFeatured = true;
            products[5].IsFeatured = true;

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFeaturedProductsAsync();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, product => Assert.True(product.IsFeatured));
            Assert.Contains(result, p => p.ProductId == products[0].ProductId);
            Assert.Contains(result, p => p.ProductId == products[2].ProductId);
            Assert.Contains(result, p => p.ProductId == products[5].ProductId);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsEmptyList_WhenNoFeaturedProductsExist()
        {
            // Arrange
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            // Ensure no products are featured
            foreach (var product in products)
            {
                product.IsFeatured = false;
            }

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFeaturedProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var products = Enumerable.Empty<Product>();

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetFeaturedProductsAsync();

            // Assert
            Assert.Empty(result);
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

            var users = new List<AppUser>();
            _mockUserManager.Setup(um => um.Users)
                .Returns(users.AsQueryable().BuildMock());

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

        #region CreateProductAsync tests

        [Fact]
        public async Task CreateProductAsync_CreatesProductAndReturnsProductId()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "Test Product",
                Price = 99.99m,
                DescriptionHtml = "<p>Test description</p>",
                WarrantyHtml = "<p>Test warranty</p>",
                PrimaryImageUrl = "https://placehold.co/600x800?text=primary",
                ImageUrls = new List<string>
                {
                    "https://placehold.co/600x800?text=1",
                    "https://placehold.co/600x800?text=2",
                },
                CategoryIds = new List<int> { 1, 2 }
            };

            Product? savedProduct = null;

            _mockProductRepository.Setup(repo => repo.AddProduct(It.IsAny<Product>()))
                .Callback<Product>(p => savedProduct = p);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1)
                .Callback(() =>
                {
                    if (savedProduct != null)
                    {
                        // Simulate the database assigning an ID
                        savedProduct.ProductId = 42;
                    }
                });

            // Act
            var result = await _service.CreateProductAsync(createProductDto);

            // Assert
            Assert.Equal(42, result);

            Assert.NotNull(savedProduct);
            Assert.Equal(createProductDto.Name, savedProduct.Name);
            Assert.Equal(createProductDto.Price, savedProduct.Price);

            Assert.Equal(createProductDto.DescriptionHtml, savedProduct.Detail.DescriptionHtml);
            Assert.Equal(createProductDto.WarrantyHtml, savedProduct.Detail.WarrantyHtml);

            // Primary + additional images
            Assert.Equal(3, savedProduct.Images.Count);

            var primaryImage = savedProduct.Images.FirstOrDefault(i => i.IsPrimary);

            Assert.NotNull(primaryImage);
            Assert.Equal(createProductDto.PrimaryImageUrl, primaryImage.Url);

            Assert.Equal(createProductDto.CategoryIds.Count, savedProduct.ProductCategories.Count);

            _mockProductRepository.Verify(repo => repo.AddProduct(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region UpdateProductAsync tests

        [Fact]
        public async Task UpdateProductAsync_UpdatesProductAndReturnsProductId_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);
            var product = products.First(p => p.ProductId == productId);

            var updateProductDto = new UpdateProductDto
            {
                ProductId = productId,
                Name = "Updated Product",
                Price = 149.99m,
                DescriptionHtml = "<p>Updated description</p>",
                WarrantyHtml = "<p>Updated warranty</p>",
                PrimaryImageUrl = "https://placehold.co/600x800",
                ImageUrls = new List<string> { "https://placehold.co/600x800" },
                CategoryIds = new List<int> { 3, 4 }
            };

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            Product? updatedProduct = null;

            _mockProductRepository.Setup(repo => repo.UpdateProduct(It.IsAny<Product>()))
                .Callback<Product>(p => updatedProduct = p);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateProductAsync(productId, updateProductDto);

            // Assert
            Assert.Equal(productId, result);

            Assert.NotNull(updatedProduct);
            Assert.Equal(updateProductDto.Name, updatedProduct.Name);
            Assert.Equal(updateProductDto.Price, updatedProduct.Price);

            Assert.Equal(updateProductDto.DescriptionHtml, updatedProduct.Detail.DescriptionHtml);
            Assert.Equal(updateProductDto.WarrantyHtml, updatedProduct.Detail.WarrantyHtml);

            // Primary + additional image
            Assert.Equal(2, updatedProduct.Images.Count);

            var primaryImage = updatedProduct.Images.FirstOrDefault(i => i.IsPrimary);

            Assert.NotNull(primaryImage);
            Assert.Equal(updateProductDto.PrimaryImageUrl, primaryImage.Url);

            Assert.Equal(2, updatedProduct.ProductCategories.Count);

            _mockProductRepository.Verify(repo => repo.UpdateProduct(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            var updateProductDto = new UpdateProductDto
            {
                ProductId = productId,
                Name = "Updated Product",
                Price = 149.99m,
                DescriptionHtml = "<p>Updated description</p>",
                WarrantyHtml = "<p>Updated warranty</p>",
                PrimaryImageUrl = "https://placehold.co/600x800",
                ImageUrls = new List<string>(),
                CategoryIds = new List<int> { 1, 2 }
            };

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.UpdateProductAsync(productId, updateProductDto);

            // Assert
            Assert.Null(result);

            _mockProductRepository.Verify(repo => repo.UpdateProduct(It.IsAny<Product>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
        }

        #endregion

        #region DeleteProductAsync tests

        [Fact]
        public async Task DeleteProductAsync_ReturnsTrue_WhenProductIsDeleted()
        {
            // Arrange
            int productId = 1;
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            Product? removedProduct = null;

            _mockProductRepository.Setup(repo => repo.RemoveProduct(It.IsAny<Product>()))
                .Callback<Product>(p => removedProduct = p);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            Assert.NotNull(removedProduct);
            Assert.Equal(productId, removedProduct.ProductId);

            _mockProductRepository.Verify(repo => repo.RemoveProduct(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentProductId = 999;
            var products = ProductGenerator.Generate(productCount: 5, categoryCount: 2);

            _mockProductRepository.Setup(repo => repo.GetQueryableProducts())
                .Returns(products.AsQueryable().BuildMock());

            // Act
            var result = await _service.DeleteProductAsync(nonExistentProductId);

            // Assert
            Assert.False(result);

            _mockProductRepository.Verify(repo => repo.RemoveProduct(It.IsAny<Product>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
        }

        #endregion
    }
}