using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using GamerCore.CustomerSite.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;

namespace GamerCore.CustomerSite.Tests.ViewComponents
{
    public class CategoryMenuViewComponentTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryMenuViewComponent _viewComponent;

        public CategoryMenuViewComponentTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _viewComponent = new CategoryMenuViewComponent(_mockCategoryService.Object);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsViewWithCategories()
        {
            // Arrange
            var categories = new List<CategoryViewModel>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Category 1" },
                new() { Id = Guid.NewGuid().ToString(), Name = "Category 2" }
            };

            _mockCategoryService.Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(viewResult.ViewData?.Model);
            Assert.Equal(categories, model);
            _mockCategoryService.Verify(s => s.GetCategoriesAsync(), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsViewWithEmptyList_WhenNoCategories()
        {
            // Arrange
            var categories = new List<CategoryViewModel>();

            _mockCategoryService.Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CategoryViewModel>>(viewResult.ViewData?.Model);
            Assert.Empty(model);
            _mockCategoryService.Verify(s => s.GetCategoriesAsync(), Times.Once);
        }
    }
}