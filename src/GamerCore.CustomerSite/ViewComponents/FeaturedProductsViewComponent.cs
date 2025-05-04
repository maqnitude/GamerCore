using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.CustomerSite.ViewComponents
{

    public class FeaturedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public FeaturedProductsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var featuredProducts = await _productService.GetFeaturedProductsAsync();
            return View(featuredProducts);
        }
    }
}