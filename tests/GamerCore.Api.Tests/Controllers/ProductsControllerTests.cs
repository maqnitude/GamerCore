using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.Api.Tests.Controllers
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

        #region GetProducts tests

        [Fact]
        public async Task GetProducts_ReturnsOk_WhenProductsExist()
        {
            // Arrange
            var product1Id = Guid.NewGuid().ToString();
            var product2Id = Guid.NewGuid().ToString();

            var productDtos = new List<ProductDto>
            {
                new() { Id = product1Id, Name = "Product 1" },
                new() { Id = product2Id, Name = "Product 2" }
            };

            var paginatedList = new PaginatedList<ProductDto>
            {
                Items = productDtos,
                Page = 1,
                PageSize = 10,
                TotalItems = 2
            };

            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<string[]?>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPaginatedList = Assert.IsAssignableFrom<PaginatedList<ProductDto>>(okResult.Value);
            Assert.Equal(paginatedList, returnedPaginatedList);

            _mockService.Verify(s => s.GetFilteredProductsAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<string[]?>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProducts_ParsesCategoryIds_Correctly()
        {
            // Arrange
            var category1Id = Guid.NewGuid().ToString();
            var category2Id = Guid.NewGuid().ToString();
            var category3Id = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<string[]?>()))
                .ReturnsAsync(new PaginatedList<ProductDto>());

            // Act
            await _controller.GetProducts(categoryIds: $"{category1Id},{category2Id},{category3Id}");

            // Assert
            _mockService.Verify(s => s.GetFilteredProductsAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.Is<string[]>(arr => arr.SequenceEqual(new[] { category1Id, category2Id, category3Id }))),
                Times.Once);
        }

        [Fact]
        public async Task GetProducts_ReturnsNoContent_WhenNoProductsExist()
        {
            // Arrange
            var emptyPaginatedList = new PaginatedList<ProductDto>
            {
                Items = new List<ProductDto>(),
                Page = 1,
                PageSize = 10,
                TotalItems = 0
            };

            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<string[]?>()))
                .ReturnsAsync(emptyPaginatedList);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetProducts_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetFilteredProductsAsync(
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<string[]?>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetProducts();

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

        #region GetFeaturedProducts tests

        [Fact]
        public async Task GetFeaturedProducts_ReturnsOk_WhenFeaturedProductsExist()
        {
            // Arrange
            var product1Id = Guid.NewGuid().ToString();
            var product2Id = Guid.NewGuid().ToString();

            var featuredProducts = new List<ProductDto>
            {
                new() { Id = product1Id, Name = "Featured Product 1", IsFeatured = true },
                new() { Id = product2Id, Name = "Featured Product 2", IsFeatured = true }
            };

            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ReturnsAsync(featuredProducts);

            // Act
            var result = await _controller.GetFeaturedProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<List<ProductDto>>(okResult.Value);
            Assert.Equal(featuredProducts, returnedProducts);

            _mockService.Verify(s => s.GetFeaturedProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFeaturedProducts_ReturnsNoContent_WhenNoFeaturedProductsExist()
        {
            // Arrange
            var emptyProductsList = new List<ProductDto>();

            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ReturnsAsync(emptyProductsList);

            // Act
            var result = await _controller.GetFeaturedProducts();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetFeaturedProducts_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetFeaturedProducts();

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

        #region GetProductDetails tests

        [Fact]
        public async Task GetProductDetails_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            var productDetailsDto = new ProductDetailsDto { Id = productId, Name = "Test Product" };

            _mockService.Setup(service => service.GetProductDetailsAsync(productId))
                .ReturnsAsync(productDetailsDto);

            // Act
            var result = await _controller.GetProductDetails(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProductDetails = Assert.IsType<ProductDetailsDto>(okResult.Value);
            Assert.Equal(productDetailsDto, returnedProductDetails);
        }

        [Fact]
        public async Task GetProductDetails_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.GetProductDetailsAsync(nonExistentProductId))
                .ReturnsAsync((ProductDetailsDto?)null);

            // Act
            var result = await _controller.GetProductDetails(nonExistentProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProductDetails_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.GetProductDetailsAsync(productId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetProductDetails(productId);

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

        #region CreateProduct tests

        [Fact]
        public async Task CreateProduct_ReturnsOk_WhenProductIsCreated()
        {
            // Arrange
            var createdProductId = Guid.NewGuid().ToString();

            var createProductDto = new CreateProductDto
            {
                Name = "New Product",
                DescriptionHtml = "<p>Description</p>",
                Price = 9.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.CreateProductAsync(It.IsAny<CreateProductDto>()))
                .ReturnsAsync(createdProductId);

            // Act
            var result = await _controller.CreateProduct(createProductDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProductDetails", createdAtActionResult.ActionName);
            Assert.Equal("Products", createdAtActionResult.ControllerName);
            Assert.Equal(createdProductId, createdAtActionResult.Value);
            Assert.Equal(createdProductId, createdAtActionResult.RouteValues?["id"]);

            _mockService.Verify(s => s.CreateProductAsync(createProductDto), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var createProductDto = new CreateProductDto();

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateProduct(createProductDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _mockService.Verify(s => s.CreateProductAsync(It.IsAny<CreateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateProduct_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "New Product",
                DescriptionHtml = "<p>Description</p>",
                Price = 9.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.CreateProductAsync(It.IsAny<CreateProductDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.CreateProduct(createProductDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
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

        #region UpdateProductAsync tests

        [Fact]
        public async Task UpdateProductAsync_ReturnsOk_WhenProductIsUpdated()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            var updateProductDto = new UpdateProductDto
            {
                Name = "Updated Product",
                DescriptionHtml = "<p>Updated Description</p>",
                Price = 19.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.UpdateProductAsync(productId, It.IsAny<UpdateProductDto>()))
                .ReturnsAsync(productId);

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(productId, updateProductDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            var updateProductDto = new UpdateProductDto();

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(It.IsAny<string>(), It.IsAny<UpdateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid().ToString();

            var updateProductDto = new UpdateProductDto
            {
                Name = "Updated Product",
                DescriptionHtml = "<p>Updated Description</p>",
                Price = 19.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.UpdateProductAsync(nonExistentProductId, It.IsAny<UpdateProductDto>()))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _controller.UpdateProduct(nonExistentProductId, updateProductDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(nonExistentProductId, updateProductDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            var updateProductDto = new UpdateProductDto
            {
                Name = "Updated Product",
                DescriptionHtml = "<p>Updated Description</p>",
                Price = 19.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.UpdateProductAsync(productId, It.IsAny<UpdateProductDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
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

        #region DeleteProduct tests

        [Fact]
        public async Task DeleteProduct_ReturnsOk_WhenProductIsDeleted()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteProductAsync(nonExistentProductId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(nonExistentProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockService.Verify(s => s.DeleteProductAsync(nonExistentProductId), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteProductAsync(productId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
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