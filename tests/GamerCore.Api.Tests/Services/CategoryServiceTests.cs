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

            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();

            foreach (var category in categories)
            {
                category.ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = category.Id, ProductId = product1Id },
                    new() { CategoryId = category.Id, ProductId = product2Id }
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
                Assert.Contains(categories, c => c.Id.ToString() == category.Id);
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

        #region GetCategoryByIdAsync tests

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory_WhenCategorieExists()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(5);
            var targetCategoryId = categories[2].Id;

            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();

            foreach (var category in categories)
            {
                category.ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = category.Id, ProductId = product1Id }
                };

                if (category.Id == targetCategoryId)
                {
                    category.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = category.Id,
                        ProductId = product2Id
                    });
                }
            }

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoryByIdAsync(targetCategoryId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetCategoryId.ToString(), result.Id);
            Assert.Equal($"Category {targetCategoryId}", result.Name);
            Assert.Equal($"Category description {targetCategoryId}", result.Description);
            Assert.Equal(2, result.ProductCount);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categories = CategoryGenerator.Generate(5);

            _mockCategoryRepository.Setup(repo => repo.GetQueryableCategories())
                .Returns(categories.AsQueryable().BuildMock());

            // Act
            var result = await _service.GetCategoryByIdAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CreateCategoryAsync tests

        [Fact]
        public async Task CreateCategoryAsync_CreatesAndReturnsCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

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
                    category.Id = categoryId;
                });

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId.ToString(), result.Id);
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
                Id = category.Id.ToString(),
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
            var result = await _service.UpdateCategoryAsync(category.Id.ToString(), updateCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Id.ToString(), result.Id);
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

            var categoryId = Guid.NewGuid().ToString();

            var updateDto = new UpdateCategoryDto
            {
                Id = categoryId,
                Name = "Non-existent Category",
                Description = "This category does not exist"
            };

            // Act
            var result = await _service.UpdateCategoryAsync(categoryId, updateDto);

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
            var result = await _service.DeleteCategoryAsync(category.Id.ToString());

            // Assert
            Assert.True(result);

            Assert.NotNull(capturedCategory);
            Assert.Equal(category.Id, capturedCategory.Id);

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
            var result = await _service.DeleteCategoryAsync(Guid.NewGuid().ToString());

            // Assert
            Assert.False(result);

            _mockCategoryRepository.Verify(repo => repo.RemoveCategory(It.IsAny<Category>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(default), Times.Never);
        }

        #endregion
    }
}