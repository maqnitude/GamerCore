using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.Api.Tests
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
    }
}