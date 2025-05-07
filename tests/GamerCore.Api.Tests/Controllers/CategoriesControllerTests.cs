using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.Api.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _mockService;
        private readonly Mock<ILogger<CategoriesController>> _mockLogger;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<CategoriesController>>();
            _controller = new CategoriesController(_mockService.Object, _mockLogger.Object);
        }

        #region GetCategoriesAsync tests

        [Fact]
        public async Task GetCategoriesAsync_ReturnsOk_WhenCategoriesExist()
        {
            // Arrange
            var categoryDtos = new List<CategoryDto>
            {
                new() { CategoryId = 1, Name = "Category 1", Description = "Description 1", ProductCount = 5 },
                new() { CategoryId = 2, Name = "Category 2", Description = "Description 2", ProductCount = 3 },
            };

            _mockService.Setup(service => service.GetCategoriesAsync())
                .ReturnsAsync(categoryDtos);

            // Act
            var result = await _controller.GetCategoriesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategoryDtos = Assert.IsAssignableFrom<List<CategoryDto>>(okResult.Value);
            Assert.Equal(categoryDtos.Count, returnedCategoryDtos.Count);
            for (int i = 0; i < categoryDtos.Count; i++)
            {
                Assert.Equal(categoryDtos[i].CategoryId, returnedCategoryDtos[i].CategoryId);
                Assert.Equal(categoryDtos[i].Name, returnedCategoryDtos[i].Name);
                Assert.Equal(categoryDtos[i].Description, returnedCategoryDtos[i].Description);
                Assert.Equal(categoryDtos[i].ProductCount, returnedCategoryDtos[i].ProductCount);
            }
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsNoContent_WhenNoCategoryExists()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoriesAsync())
                .ReturnsAsync(new List<CategoryDto>());

            // Act
            var result = await _controller.GetCategoriesAsync();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoriesAsync())
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetCategoriesAsync();

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

        #region GetCategoryByIdAsync tests

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsOk_WhenCategoryExists()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                CategoryId = 1,
                Name = "Category 1",
                Description = "Description 1",
                ProductCount = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            _mockService.Setup(service => service.GetCategoryByIdAsync(1)).ReturnsAsync(categoryDto);

            // Act
            var result = await _controller.GetCategoryByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(categoryDto.CategoryId, returnedCategory.CategoryId);
            Assert.Equal(categoryDto.Name, returnedCategory.Name);
            Assert.Equal(categoryDto.Description, returnedCategory.Description);
            Assert.Equal(categoryDto.ProductCount, returnedCategory.ProductCount);
            Assert.Equal(categoryDto.CreatedAt, returnedCategory.CreatedAt);
            Assert.Equal(categoryDto.UpdatedAt, returnedCategory.UpdatedAt);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoryByIdAsync(999))
                .ReturnsAsync((CategoryDto?)null);

            // Act
            var result = await _controller.GetCategoryByIdAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoryByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetCategoryByIdAsync(1);

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

        #region CreateCategoryAsync tests

        [Fact]
        public async Task CreateCategoryAsync_ReturnsCreated_WhenCategoryIsCreated()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "New Category",
                Description = "New Category Description"
            };

            var createdCategoryDto = new CategoryDto
            {
                CategoryId = 1,
                Name = "New Category",
                Description = "New Category Description",
                ProductCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockService.Setup(service => service.CreateCategoryAsync(It.IsAny<CreateCategoryDto>()))
                .ReturnsAsync(createdCategoryDto);

            // Act
            var result = await _controller.CreateCategoryAsync(createCategoryDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCategoryById", createdAtActionResult.ActionName);
            Assert.Equal("Categories", createdAtActionResult.ControllerName);
            Assert.Equal(createdCategoryDto.CategoryId, createdAtActionResult.RouteValues?["id"]);

            var returnedCategory = Assert.IsType<CategoryDto>(createdAtActionResult.Value);
            Assert.Equal(createdCategoryDto.CategoryId, returnedCategory.CategoryId);
            Assert.Equal(createCategoryDto.Name, returnedCategory.Name);
            Assert.Equal(createCategoryDto.Description, returnedCategory.Description);
        }

        [Fact]
        public async Task CreateCategoryAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "",
                Description = "Description"
            };

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "New Category",
                Description = "New Category Description"
            };

            _mockService.Setup(service => service.CreateCategoryAsync(It.IsAny<CreateCategoryDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.CreateCategoryAsync(createCategoryDto);

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

        #region UpdateCategoryAsync tests

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsOk_WhenCategoryIsUpdated()
        {
            // Arrange
            int categoryId = 1;
            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = categoryId,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            var updatedCategoryDto = new CategoryDto
            {
                CategoryId = categoryId,
                Name = "Updated Category",
                Description = "Updated Description",
                ProductCount = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ReturnsAsync(updatedCategoryDto);

            // Act
            var result = await _controller.UpdateCategoryAsync(categoryId, updateCategoryDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(updatedCategoryDto.CategoryId, returnedCategory.CategoryId);
            Assert.Equal(updateCategoryDto.Name, returnedCategory.Name);
            Assert.Equal(updateCategoryDto.Description, returnedCategory.Description);
            Assert.Equal(updatedCategoryDto.ProductCount, returnedCategory.ProductCount);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int categoryId = 1;
            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = categoryId,
                Name = "",
                Description = "Updated Description"
            };

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateCategoryAsync(categoryId, updateCategoryDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 999;
            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = categoryId,
                Name = "Non-existent Category",
                Description = "This category does not exist"
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ReturnsAsync((CategoryDto?)null);

            // Act
            var result = await _controller.UpdateCategoryAsync(categoryId, updateCategoryDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int categoryId = 1;
            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = categoryId,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.UpdateCategoryAsync(categoryId, updateCategoryDto);

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

        #region DeleteCategoryAsync tests

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsNoContent_WhenCategoryIsDeleted()
        {
            // Arrange
            int categoryId = 1;

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 999;

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int categoryId = 1;

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.DeleteCategoryAsync(categoryId);

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