using GamerCore.Api.Controllers;
using GamerCore.Api.Models;
using GamerCore.Api.Tests.Utilities;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GamerCore.Api.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryRepository> _mockRepository;
        private readonly Mock<ILogger<CategoriesController>> _mockLogger;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _mockLogger = new Mock<ILogger<CategoriesController>>();
            _controller = new CategoriesController(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsOk_WhenCategoriesExist()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(categoryCount: 10);

            _mockRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetCategoriesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var categoryDtos = Assert.IsAssignableFrom<List<CategoryDto>>(okResult.Value);
            Assert.Equal(categories.Count, categoryDtos.Count);

            for (int i = 0; i < categories.Count; i++)
            {
                Assert.Equal(categories[i].Name, categoryDtos[i].Name);
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
        public async Task GetCategoriesAsync_ReturnsNoContent_WhenNoCategoryExists()
        {
            // Arrange
            var categories = Enumerable.Empty<Category>();

            _mockRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _controller.GetCategoriesAsync();

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
        public async Task GetCategoriesAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetQueryableCategories())
                .Throws(new Exception("Test Exception"));

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