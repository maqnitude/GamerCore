using GamerCore.Core.Models;
using GamerCore.CustomerSite.Controllers;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.CustomerSite.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockCategoryService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(
                _mockProductService.Object,
                _mockCategoryService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithProducts_WhenNoCategorySelected()
        {
            // Arrange
            var expectedProducts = new PagedResult<ProductViewModel>
            {
                Items = [new() { ProductId = 1, Name = "Test Product", Price = 10.00M }],
                Page = 1,
                PageSize = 10,
                TotalItems = 1
            };

            var expectedCategories = new List<CategoryViewModel>
            {
                new() { CategoryId = 1, Name = "Test Category" }
            };

            _mockProductService
                .Setup(s => s.GetProductsAsync(1, null))
                .ReturnsAsync(expectedProducts);

            _mockCategoryService
                .Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _controller.Index(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductListViewModel>(viewResult.Model);

            Assert.Equal(expectedProducts.Items, model.Products);
            Assert.Equal(expectedCategories, model.Filter.Categories);
            Assert.Equal(1, model.Pagination.Page);
            Assert.Equal(10, model.Pagination.PageSize);
            Assert.Equal(1, model.Pagination.TotalItems);
            Assert.Null(model.Filter.SelectedCategoryId);
        }

        [Fact]
        public async Task Index_ReturnsViewWithFilteredProducts_WhenCategorySelected()
        {
            // Arrange
            int categoryId = 1;
            var category = new CategoryViewModel
            {
                CategoryId = categoryId,
                Name = "Test Category"
            };

            var expectedProducts = new PagedResult<ProductViewModel>
            {
                Items =
                [
                    new()
                    {
                        ProductId = 1,
                        Name = "Test Product",
                        Price = 10.00M,
                        Categories = [category]
                    }
                ],
                Page = 1,
                PageSize = 10,
                TotalItems = 1
            };

            var expectedCategories = new List<CategoryViewModel>
            {
                category
            };

            _mockProductService
                .Setup(s => s.GetProductsAsync(1, new[] { categoryId }))
                .ReturnsAsync(expectedProducts);

            _mockCategoryService
                .Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _controller.Index(1, categoryId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductListViewModel>(viewResult.Model);

            Assert.Equal(expectedProducts.Items, model.Products);
            Assert.Equal(expectedCategories, model.Filter.Categories);
            Assert.Equal(1, model.Pagination.Page);
            Assert.Equal(categoryId, model.Filter.SelectedCategoryId);

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
        public async Task Index_RedirectsToErrorAction_WhenExceptionOccurs()
        {
            // Arrange
            _mockProductService
                .Setup(s => s.GetProductsAsync(It.IsAny<int>(), null))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.Index(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);

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