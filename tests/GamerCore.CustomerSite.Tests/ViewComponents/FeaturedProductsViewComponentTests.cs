using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using GamerCore.CustomerSite.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;

namespace GamerCore.CustomerSite.Tests.ViewComponents
{
    public class FeaturedProductsViewComponentTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private FeaturedProductsViewComponent _viewComponent;

        public FeaturedProductsViewComponentTests()
        {
            _mockProductService = new Mock<IProductService>();
            _viewComponent = new FeaturedProductsViewComponent(_mockProductService.Object);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsViewWithFeaturedProducts_WhenFeaturedProductsExist()
        {
            // Arrange
            var featuredProducts = new List<ProductViewModel>
            {
                new() { ProductId = 1, Name = "Product 1", IsFeatured = true },
                new() { ProductId = 2, Name = "Product 2", IsFeatured = true }
            };

            _mockProductService.Setup(s => s.GetFeaturedProductsAsync())
                .ReturnsAsync(featuredProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData?.Model);
            Assert.Equal(featuredProducts, model);
            _mockProductService.Verify(s => s.GetFeaturedProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsViewWithEmptyList_WhenNoFeaturedProductsExist()
        {
            // Arrange
            var featuredProducts = new List<ProductViewModel>();
            _mockProductService.Setup(s => s.GetFeaturedProductsAsync())
                             .ReturnsAsync(featuredProducts);

            // Act
            var result = await _viewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData?.Model);
            Assert.Empty(model);
            _mockProductService.Verify(s => s.GetFeaturedProductsAsync(), Times.Once);
        }
    }
}