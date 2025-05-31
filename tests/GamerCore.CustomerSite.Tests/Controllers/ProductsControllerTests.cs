using GamerCore.Core.Models;
using GamerCore.CustomerSite.Controllers;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.CustomerSite.Tests.Controllers
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

        #region Index tests

        [Fact]
        public async Task Index_ReturnsViewWithProducts_WhenNoFiltersAreApplied()
        {
            // Arrange
            var expectedProducts = new PaginatedList<ProductViewModel>
            {
                Items = [new() { Id = Guid.NewGuid().ToString(), Name = "Test Product", Price = 10.00M }],
                Page = 1,
                PageSize = 10,
                TotalItems = 1
            };

            var expectedCategoryId = Guid.NewGuid();

            var expectedCategories = new List<CategoryViewModel>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Test Category" }
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
        public async Task Index_ReturnsViewWithFilteredProducts_WhenCategoryFilterIsApplied()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();

            var category = new CategoryViewModel
            {
                Id = categoryId,
                Name = "Test Category"
            };

            var expectedProducts = new PaginatedList<ProductViewModel>
            {
                Items =
                [
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
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
        }

        [Fact]
        public async Task Index_RedirectsToErrorAction_WhenExceptionIsThrown()
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
            Assert.Equal("Home", redirectResult.ControllerName);

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

        #region Details tests

        [Fact]
        public async Task Details_ReturnsViewWithProductDetails_WhenRetrievalSucceeds()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            var expectedProductDetails = new ProductDetailsViewModel
            {
                Id = productId,
                Name = "Test product",
                Price = 49.99M,
                DescriptionHtml = "<p>Test product description</p>",
                WarrantyHtml = "<p>Test product warrenty</p>",
                Categories = new List<CategoryViewModel>
                {
                    new() { Id = Guid.NewGuid().ToString(), Name = "Category 1" }
                },
                Images = new List<ProductImageViewModel>
                {
                    new() { Id = Guid.NewGuid().ToString(), Url = "https://placehold.co/600x400", IsPrimary = true }
                },
                AverageRating = 4.5,
                ReviewCount = 10,
                Reviews = new List<ProductReviewViewModel>
                {
                    new() { Id = Guid.NewGuid().ToString(), Rating = 5, ReviewText = "Product review text" }
                }
            };

            _mockProductService
                .Setup(s => s.GetProductDetailsAsync(productId))
                .ReturnsAsync(expectedProductDetails);

            // Act
            var result = await _controller.Details(productId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDetailsViewModel>(viewResult.Model);

            Assert.Equal(expectedProductDetails, model);
            Assert.Equal(productId, model.Id);
            Assert.Equal("Test product", model.Name);
            Assert.Equal(49.99M, model.Price);
            Assert.Equal("<p>Test product description</p>", model.DescriptionHtml);
            Assert.Equal("<p>Test product warrenty</p>", model.WarrantyHtml);
            Assert.Single(model.Categories);
            Assert.Single(model.Images);
            Assert.Equal(4.5, model.AverageRating);
            Assert.Equal(10, model.ReviewCount);
            Assert.Single(model.Reviews);
        }

        [Fact]
        public async Task Details_RedirectsToErrorAction_WhenExceptionIsThrown()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();

            _mockProductService
                .Setup(s => s.GetProductDetailsAsync(productId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.Details(productId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

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