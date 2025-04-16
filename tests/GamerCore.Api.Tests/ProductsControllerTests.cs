using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
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
        [Fact]
        public async Task GetProductsAsync_ReturnsOk_WithDefaultPagination_WhenProductsExist()
        {
            // Arrange
            var products = GetTestProductList();

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            var productDtos = pagedResult.Items;
            Assert.Equal(products.Count, productDtos.Count);

            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(products[i].ProductId, productDtos[i].ProductId);
                Assert.Equal(products[i].Name, productDtos[i].Name);
                Assert.Equal(products[i].Description, productDtos[i].Description);
                Assert.Equal(products[i].Price, productDtos[i].Price);
                Assert.Equal(products[i].ProductCategories.First().Category.Name, productDtos[i].Categories.First().Name);
            }

            mockLogger.Verify(
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
            var products = GetLargeTestProductList(count: 20);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProductsAsync(page: 2, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResult.Value);
            var productDtos = pagedResult.Items;

            Assert.Equal(5, productDtos.Count);
            Assert.Equal(6, productDtos[0].ProductId);
            Assert.Equal(10, productDtos[^1].ProductId);

            mockLogger.Verify(
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
            var products = GetLargeTestProductList(count: 20);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var resultLo = await controller.GetProductsAsync(page: -1, pageSize: 5);
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
            var products = GetLargeTestProductList(count: 20);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var resultLo = await controller.GetProductsAsync(page: 1, pageSize: 0);
            var resultHi = await controller.GetProductsAsync(page: 1, pageSize: 999);

            // Assert
            var okResultLo = Assert.IsType<OkObjectResult>(resultLo.Result);
            var okResultHi = Assert.IsType<OkObjectResult>(resultHi.Result);
            var pagedResultLo = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultLo.Value);
            var pagedResultHi = Assert.IsAssignableFrom<PagedResult<ProductDto>>(okResultHi.Value);
            Assert.Equal(1, pagedResultLo.PageSize);
            Assert.Equal(PaginationConstants.MaxPageSize, pagedResultHi.PageSize);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsNoContent_WhenNoProductsExist()
        {
            // Arrange
            var products = Enumerable.Empty<Product>();

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProductsAsync();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);

            mockLogger.Verify(
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
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Throws(new Exception("Test Exception"));

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProductsAsync();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal Server Error", objectResult.Value);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        private static List<Product> GetTestProductList()
        {
            var category1 = new Category { CategoryId = 101, Name = "Category 1" };
            var category2 = new Category { CategoryId = 102, Name = "Category 2" };

            var product1 = new Product
            {
                ProductId = 1,
                Name = "Product 1",
                Description = "Description 1",
                Price = 10.00m
            };
            var product2 = new Product
            {
                ProductId = 2,
                Name = "Product 2",
                Description = "Description 2",
                Price = 20.00m
            };

            product1.ProductCategories = new List<ProductCategory>
            {
                new()
                {
                    ProductId = 1,
                    Product = product1,
                    CategoryId = 101,
                    Category = category1
                }
            };
            product2.ProductCategories = new List<ProductCategory>
            {
                new()
                {
                    ProductId = 2,
                    Product = product2,
                    CategoryId = 102,
                    Category = category2
                }
            };

            return new List<Product> { product1, product2 };
        }

        private static List<Product> GetLargeTestProductList(int count = 20)
        {
            var products = new List<Product>();
            var category = new Category { CategoryId = 100, Name = "Category" };

            for (int i = 1; i <= count; i++)
            {
                var product = new Product
                {
                    ProductId = i,
                    Name = $"Product {i}",
                    Description = $"Description {i}",
                    Price = i * 10.00m
                };

                product.ProductCategories.Add(new ProductCategory
                {
                    ProductId = i,
                    Product = product,
                    CategoryId = 100,
                    Category = category
                });

                products.Add(product);
            }

            return products;
        }
    }
}