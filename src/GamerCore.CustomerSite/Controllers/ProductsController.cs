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

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: /Products?page=1&categoryId=<guid>
        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] string? categoryId = null)
        {
            try
            {
                var paginatedListTask = categoryId != null
                    ? _productService.GetProductsAsync(page, [categoryId])
                    : _productService.GetProductsAsync(page);
                var categoriesTask = _categoryService.GetCategoriesAsync();

                await Task.WhenAll(paginatedListTask, categoriesTask);

                var paginatedList = await paginatedListTask;
                var categories = await categoriesTask;

                categories = categories.OrderBy(c => c.Name).ToList();

                var productFilter = new ProductFilterViewModel()
                {
                    Categories = categories,
                    SelectedCategoryId = categoryId
                };

                var productListViewModel = new ProductListViewModel
                {
                    Products = paginatedList.Items,
                    Filter = productFilter,
                    Pagination = new PaginationMetadata()
                    {
                        Page = paginatedList.Page,
                        PageSize = paginatedList.PageSize,
                        TotalItems = paginatedList.TotalItems
                    }
                };

                _logger.LogInformation("Successfully displayed the products page.");
                return View(productListViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while displaying the products page.");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: /Products/Details/<guid>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var productDetails = await _productService.GetProductDetailsAsync(id);

                return View(productDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while displaying the product details page.");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}