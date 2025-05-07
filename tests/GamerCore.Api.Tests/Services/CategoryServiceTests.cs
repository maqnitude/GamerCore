using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Api.Tests.Utilities;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GamerCore.Api.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ILogger<CategoryService>> _mockLogger;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CategoryService>>();

            _mockUnitOfWork.SetupGet(uow => uow.Products).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.SetupGet(uow => uow.Categories).Returns(_mockCategoryRepository.Object);

            _service = new CategoryService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        #region GetCategoriesAsync tests

        [Fact]
        public async Task GetCategoriesAsync_ReturnsCategoryList_WhenCategoriesExist()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(5);
            foreach (var category in categories)
            {
                category.ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = category.CategoryId, ProductId = 1 },
                    new() { CategoryId = category.CategoryId, ProductId = 2 }
                };
            }

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);

            foreach (var category in result)
            {
                Assert.Equal(2, category.ProductCount);
                Assert.Contains(categories, c => c.CategoryId == category.CategoryId);
                Assert.Contains(categories, c => c.Name == category.Name);
                Assert.Contains(categories, c => c.Description == category.Description);
            }
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var emptyCategories = new List<Category>();

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(emptyCategories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetCategoryAsync tests

        [Fact]
        public async Task GetCategoryAsync_ReturnsCategory_WhenCategorieExists()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(5);
            int targetCategoryId = 3;

            foreach (var category in categories)
            {
                category.ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = category.CategoryId, ProductId = 1 }
                };

                if (category.CategoryId == targetCategoryId)
                {
                    category.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = category.CategoryId,
                        ProductId = 2
                    });
                }
            }

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoryByIdAsync(targetCategoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetCategoryId, result.CategoryId);
            Assert.Equal($"Category {targetCategoryId}", result.Name);
            Assert.Equal($"Category description {targetCategoryId}", result.Description);
            Assert.Equal(2, result.ProductCount);
        }

        [Fact]
        public async Task GetCategoryAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(5);

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoryByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CreateCategoryAsync tests

        [Fact]
        public async Task CreateCategoryAsync_CreatesAndReturnsCategory()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "New category",
                Description = "New category description"
            };

            Category? capturedCategory = null;

            _mockCategoryRepository.Setup(repo => repo.AddCategory(It.IsAny<Category>()))
                .Callback<Category>(category =>
                {
                    capturedCategory = category;
                    // Simulate database setting the ID
                    category.CategoryId = 1;
                });

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CategoryId);
            Assert.Equal(createCategoryDto.Name, result.Name);
            Assert.Equal(createCategoryDto.Description, result.Description);

            Assert.NotNull(capturedCategory);
            Assert.Equal(createCategoryDto.Name, capturedCategory.Name);
            Assert.Equal(createCategoryDto.Description, capturedCategory.Description);

            _mockCategoryRepository.Verify(repo => repo.AddCategory(It.IsAny<Category>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region UpdateCategoryAsync tests

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesAndReturnsCategory()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(1);
            var category = categories.First();

            var updateCategoryDto = new UpdateCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = "Updated Category Name",
                Description = "Updated Category Description"
            };

            var createdAt = DateTime.UtcNow.AddDays(-10);
            var originalUpdatedAt = DateTime.UtcNow.AddDays(-5);

            category.CreatedAt = createdAt;
            category.UpdatedAt = originalUpdatedAt;

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            Category? capturedCategory = null;

            _mockCategoryRepository.Setup(repo => repo.UpdateCategory(It.IsAny<Category>()))
                .Callback<Category>(c =>
                {
                    capturedCategory = c;
                    // Simulate database updating the timestamp
                    c.UpdatedAt = DateTime.UtcNow;
                });

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateCategoryAsync(category.CategoryId, updateCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.CategoryId, result.CategoryId);
            Assert.Equal(updateCategoryDto.Name, result.Name);
            Assert.Equal(updateCategoryDto.Description, result.Description);
            Assert.Equal(createdAt, result.CreatedAt);
            Assert.NotEqual(originalUpdatedAt, result.UpdatedAt);

            Assert.NotNull(capturedCategory);
            Assert.Equal(updateCategoryDto.Name, capturedCategory.Name);
            Assert.Equal(updateCategoryDto.Description, capturedCategory.Description);

            _mockCategoryRepository.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var emptyCategories = new List<Category>();

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(emptyCategories.AsQueryable().BuildMock());

            var updateDto = new UpdateCategoryDto
            {
                CategoryId = 999,
                Name = "Non-existent Category",
                Description = "This category does not exist"
            };

            // Act
            var result = await _service.UpdateCategoryAsync(999, updateDto);

            // Assert
            Assert.Null(result);

            _mockCategoryRepository.Verify(repo => repo.UpdateCategory(It.IsAny<Category>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
        }

        #endregion

        #region DeleteCategoryAsync tests

        [Fact]
        public async Task DeleteCategoryAsync_DeletesAndReturnsTrue()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(1);
            var category = categories.First();

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            Category? capturedCategory = null;

            _mockCategoryRepository.Setup(repo => repo.RemoveCategory(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteCategoryAsync(category.CategoryId);

            // Assert
            Assert.True(result);

            Assert.NotNull(capturedCategory);
            Assert.Equal(category.CategoryId, capturedCategory.CategoryId);

            _mockCategoryRepository.Verify(repo => repo.RemoveCategory(It.IsAny<Category>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var emptyCategories = new List<Category>();

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(emptyCategories.AsQueryable().BuildMock());

            // Act
            var result = await _service.DeleteCategoryAsync(999);

            // Assert
            Assert.False(result);

            _mockCategoryRepository.Verify(repo => repo.RemoveCategory(It.IsAny<Category>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
        }

        #endregion
    }
}