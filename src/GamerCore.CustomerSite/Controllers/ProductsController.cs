using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.CustomerSite.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ICategoryService categoryService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var pagedResult = categoryId != null
                    ? await _productService.GetProductsAsync(page, [categoryId.Value])
                    : await _productService.GetProductsAsync(page);
                var categories = await _categoryService.GetCategoriesAsync();

                var productFilter = new ProductFilterViewModel()
                {
                    Categories = categories,
                    SelectedCategoryId = categoryId
                };

                var productListViewModel = new ProductListViewModel()
                {
                    Products = pagedResult.Items,
                    Filter = productFilter,
                    Pagination = new PaginationMetadata()
                    {
                        Page = pagedResult.Page,
                        PageSize = pagedResult.PageSize,
                        TotalItems = pagedResult.TotalItems
                    }
                };

                return View(productListViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while displaying the products page {page}.");
                return RedirectToAction("Error");
            }
        }
    }
}