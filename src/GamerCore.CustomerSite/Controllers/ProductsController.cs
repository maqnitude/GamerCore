using System.Text.Json;
using GamerCore.CustomerSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.CustomerSite.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IHttpClientFactory httpClientFactory, ILogger<ProductsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("GamerCoreDev");
                var response = await client.GetAsync("/api/Products");

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var products = await JsonSerializer
                        .DeserializeAsync<IEnumerable<ProductViewModel>>(contentStream) ?? [];

                    var productListViewModel = new ProductListViewModel()
                    {
                        Products = products
                    };

                    _logger.LogInformation($"[STATUS CODE {response.StatusCode}] Successfully retrieved {products.Count()} products.");
                    return View(productListViewModel);
                }
                else
                {
                    _logger.LogError($"[STATUS CODE {response.StatusCode}] Failed to retrieve products.");
                    return RedirectToAction("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Request failed.");
                return RedirectToAction("Error");
            }
        }
    }
}