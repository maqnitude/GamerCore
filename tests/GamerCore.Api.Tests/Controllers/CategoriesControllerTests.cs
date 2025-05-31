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

        #region GetCategories tests

        [Fact]
        public async Task GetCategories_ReturnsOk_WhenCategoriesExist()
        {
            // Arrange
            var category1Id = Guid.NewGuid().ToString();
            var category2Id = Guid.NewGuid().ToString();

            var categoryDtos = new List<CategoryDto>
            {
                new() { Id = category1Id, Name = "Category 1", Description = "Description 1", ProductCount = 5 },
                new() { Id = category2Id, Name = "Category 2", Description = "Description 2", ProductCount = 3 },
            };

            _mockService.Setup(service => service.GetCategoriesAsync())
                .ReturnsAsync(categoryDtos);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategoryDtos = Assert.IsAssignableFrom<List<CategoryDto>>(okResult.Value);
            Assert.Equal(categoryDtos.Count, returnedCategoryDtos.Count);
            for (int i = 0; i < categoryDtos.Count; i++)
            {
                Assert.Equal(categoryDtos[i].Id, returnedCategoryDtos[i].Id);
                Assert.Equal(categoryDtos[i].Name, returnedCategoryDtos[i].Name);
                Assert.Equal(categoryDtos[i].Description, returnedCategoryDtos[i].Description);
                Assert.Equal(categoryDtos[i].ProductCount, returnedCategoryDtos[i].ProductCount);
            }
        }

        [Fact]
        public async Task GetCategories_ReturnsNoContent_WhenNoCategoryExists()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoriesAsync())
                .ReturnsAsync(new List<CategoryDto>());

            // Act
            var result = await _controller.GetCategories();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetCategories_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetCategoriesAsync())
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetCategories();

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

        #region GetCategoryById tests

        [Fact]
        public async Task GetCategoryById_ReturnsOk_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var categoryDto = new CategoryDto
            {
                Id = categoryId,
                Name = "Category 1",
                Description = "Description 1",
                ProductCount = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            _mockService.Setup(service => service.GetCategoryByIdAsync(categoryId)).ReturnsAsync(categoryDto);

            // Act
            var result = await _controller.GetCategoryById(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(categoryDto.Id, returnedCategory.Id);
            Assert.Equal(categoryDto.Name, returnedCategory.Name);
            Assert.Equal(categoryDto.Description, returnedCategory.Description);
            Assert.Equal(categoryDto.ProductCount, returnedCategory.ProductCount);
            Assert.Equal(categoryDto.CreatedAt, returnedCategory.CreatedAt);
            Assert.Equal(categoryDto.UpdatedAt, returnedCategory.UpdatedAt);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.GetCategoryByIdAsync(categoryId))
                .ReturnsAsync((CategoryDto?)null);

            // Act
            var result = await _controller.GetCategoryById(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.GetCategoryByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetCategoryById(categoryId);

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

        #region CreateCategory tests

        [Fact]
        public async Task CreateCategory_ReturnsCreated_WhenCategoryIsCreated()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var createCategoryDto = new CreateCategoryDto
            {
                Name = "New Category",
                Description = "New Category Description"
            };

            var createdCategoryDto = new CategoryDto
            {
                Id = categoryId,
                Name = "New Category",
                Description = "New Category Description",
                ProductCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockService.Setup(service => service.CreateCategoryAsync(It.IsAny<CreateCategoryDto>()))
                .ReturnsAsync(createdCategoryDto);

            // Act
            var result = await _controller.CreateCategory(createCategoryDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCategoryById", createdAtActionResult.ActionName);
            Assert.Equal("Categories", createdAtActionResult.ControllerName);
            Assert.Equal(createdCategoryDto.Id, createdAtActionResult.RouteValues?["id"]);

            var returnedCategory = Assert.IsType<CategoryDto>(createdAtActionResult.Value);
            Assert.Equal(createdCategoryDto.Id, returnedCategory.Id);
            Assert.Equal(createCategoryDto.Name, returnedCategory.Name);
            Assert.Equal(createCategoryDto.Description, returnedCategory.Description);
        }

        [Fact]
        public async Task CreateCategory_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "",
                Description = "Description"
            };

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.CreateCategory(createCategoryDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsInternalServerError_WhenExceptionIsThrown()
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
            var result = await _controller.CreateCategory(createCategoryDto);

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

        #region UpdateCategory tests

        [Fact]
        public async Task UpdateCategory_ReturnsOk_WhenCategoryIsUpdated()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var updateCategoryDto = new UpdateCategoryDto
            {
                Id = categoryId,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            var updatedCategoryDto = new CategoryDto
            {
                Id = categoryId,
                Name = "Updated Category",
                Description = "Updated Description",
                ProductCount = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ReturnsAsync(updatedCategoryDto);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateCategoryDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(updatedCategoryDto.Id, returnedCategory.Id);
            Assert.Equal(updateCategoryDto.Name, returnedCategory.Name);
            Assert.Equal(updateCategoryDto.Description, returnedCategory.Description);
            Assert.Equal(updatedCategoryDto.ProductCount, returnedCategory.ProductCount);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var updateCategoryDto = new UpdateCategoryDto
            {
                Id = categoryId,
                Name = "",
                Description = "Updated Description"
            };

            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateCategoryDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var updateCategoryDto = new UpdateCategoryDto
            {
                Id = categoryId,
                Name = "Non-existent Category",
                Description = "This category does not exist"
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ReturnsAsync((CategoryDto?)null);

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateCategoryDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var updateCategoryDto = new UpdateCategoryDto
            {
                Id = categoryId,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            _mockService.Setup(service => service.UpdateCategoryAsync(categoryId, It.IsAny<UpdateCategoryDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.UpdateCategory(categoryId, updateCategoryDto);

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

        #region DeleteCategory tests

        [Fact]
        public async Task DeleteCategory_ReturnsNoContent_WhenCategoryIsDeleted()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            _mockService.Setup(service => service.DeleteCategoryAsync(categoryId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.DeleteCategory(categoryId);

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