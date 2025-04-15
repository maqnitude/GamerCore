using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Core.Entities;
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
        public async Task GetProductsAsync_ReturnsOk_WhenProductsExist()
        {
            // Arrange
            var products = GetTestProducts();

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetQueryableProducts()).Returns(products.AsQueryable().BuildMock());

            var mockLogger = new Mock<ILogger<ProductsController>>();

            var controller = new ProductsController(mockRepository.Object, mockLogger.Object);

            // Act
            var result = await controller.GetProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var productsDtos = Assert.IsAssignableFrom<List<ProductDto>>(okResult.Value);
            Assert.Equal(products.Count, productsDtos.Count);

            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(products[i].ProductId, productsDtos[i].ProductId);
                Assert.Equal(products[i].Name, productsDtos[i].Name);
                Assert.Equal(products[i].Description, productsDtos[i].Description);
                Assert.Equal(products[i].Price, productsDtos[i].Price);
                Assert.Equal(products[i].ProductCategories.First().Category.Name, productsDtos[i].Categories.First().Name);
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

        private static List<Product> GetTestProducts()
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
    }
}