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
                    It.IsAny<int[]?>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPaginatedList = Assert.IsAssignableFrom<PaginatedList<ProductDto>>(okResult.Value);
            Assert.Equal(paginatedList, returnedPaginatedList);

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
                    It.IsAny<int[]?>()))
                .ReturnsAsync(emptyPaginatedList);

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

        #region GetFeaturedProductsAsync tests

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsOk_WhenFeaturedProductsExist()
        {
            // Arrange
            var featuredProducts = new List<ProductDto>
            {
                new() { ProductId = 1, Name = "Featured Product 1", IsFeatured = true },
                new() { ProductId = 2, Name = "Featured Product 2", IsFeatured = true }
            };

            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ReturnsAsync(featuredProducts);

            // Act
            var result = await _controller.GetFeaturedProductsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<List<ProductDto>>(okResult.Value);
            Assert.Equal(featuredProducts, returnedProducts);

            _mockService.Verify(s => s.GetFeaturedProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsNoContent_WhenNoFeaturedProductsExist()
        {
            // Arrange
            var emptyProductsList = new List<ProductDto>();

            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ReturnsAsync(emptyProductsList);

            // Act
            var result = await _controller.GetFeaturedProductsAsync();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetFeaturedProductsAsync())
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetFeaturedProductsAsync();

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

        #region CreateProductAsync tests

        [Fact]
        public async Task CreateProductAsync_ReturnsOk_WhenProductIsCreated()
        {
            // Arrange
            int createdProductId = 1;
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
            var result = await _controller.CreateProductAsync(createProductDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProductDetails", createdAtActionResult.ActionName);
            Assert.Equal("Products", createdAtActionResult.ControllerName);
            Assert.Equal(createdProductId, createdAtActionResult.Value);
            Assert.Equal(createdProductId, createdAtActionResult.RouteValues?["id"]);

            _mockService.Verify(s => s.CreateProductAsync(createProductDto), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var createProductDto = new CreateProductDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateProductAsync(createProductDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _mockService.Verify(s => s.CreateProductAsync(It.IsAny<CreateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateProductAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
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
            var result = await _controller.CreateProductAsync(createProductDto);

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
            int productId = 1;
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
            var result = await _controller.UpdateProductAsync(productId, updateProductDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(productId, updateProductDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int productId = 1;
            var updateProductDto = new UpdateProductDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateProductAsync(productId, updateProductDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(It.IsAny<int>(), It.IsAny<UpdateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentProductId = 999;
            var updateProductDto = new UpdateProductDto
            {
                Name = "Updated Product",
                DescriptionHtml = "<p>Updated Description</p>",
                Price = 19.99m,
                PrimaryImageUrl = "https://placehold.co/600x800"
            };

            _mockService.Setup(service => service.UpdateProductAsync(nonExistentProductId, It.IsAny<UpdateProductDto>()))
                .ReturnsAsync((int?)null);

            // Act
            var result = await _controller.UpdateProductAsync(nonExistentProductId, updateProductDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockService.Verify(s => s.UpdateProductAsync(nonExistentProductId, updateProductDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int productId = 1;
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
            var result = await _controller.UpdateProductAsync(productId, updateProductDto);

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

        #region DeleteProductAsync tests

        [Fact]
        public async Task DeleteProductAsync_ReturnsOk_WhenProductIsDeleted()
        {
            // Arrange
            int productId = 1;

            _mockService.Setup(service => service.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProductAsync(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistentProductId = 999;

            _mockService.Setup(service => service.DeleteProductAsync(nonExistentProductId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProductAsync(nonExistentProductId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _mockService.Verify(s => s.DeleteProductAsync(nonExistentProductId), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int productId = 1;

            _mockService.Setup(service => service.DeleteProductAsync(productId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.DeleteProductAsync(productId);

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