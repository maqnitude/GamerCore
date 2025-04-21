using System.Threading.Tasks;
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

        // GET: Products
        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var pagedResultTask = categoryId != null
                    ? _productService.GetProductsAsync(page, [categoryId.Value])
                    : _productService.GetProductsAsync(page);
                var categoriesTask = _categoryService.GetCategoriesAsync();

                await Task.WhenAll(pagedResultTask, categoriesTask);

                var pagedResult = await pagedResultTask;
                var categories = await categoriesTask;

                categories = categories.OrderBy(c => c.Name).ToList();

                var productFilter = new ProductFilterViewModel()
                {
                    Categories = categories,
                    SelectedCategoryId = categoryId
                };

                var productListViewModel = new ProductListViewModel
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

                _logger.LogInformation("Successfully displayed the products page.");
                return View(productListViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while displaying the products page.");
                return RedirectToAction("Error");
            }
        }

        // GET: /Products/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var productDetails = await _productService.GetProductDetailsAsync(id);

            return View(productDetails);
        }
    }
}