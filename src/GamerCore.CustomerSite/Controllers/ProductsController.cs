using System.Text.Json;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                string apiUrl = "/api/Products/";

                if (page > 1)
                {
                    apiUrl += $"Page/{page}";
                }

                var client = _httpClientFactory.CreateClient("GamerCoreDev");
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var pagedResult = await JsonSerializer
                        .DeserializeAsync<PagedResult<ProductViewModel>>(contentStream) ?? new();

                    var productListViewModel = new ProductListViewModel()
                    {
                        Products = pagedResult.Items,
                        Pagination = new PaginationMetadata()
                        {
                            Page = pagedResult.Page,
                            PageSize = pagedResult.PageSize,
                            TotalItems = pagedResult.TotalItems
                        }
                    };

                    _logger.LogInformation($"[STATUS CODE {response.StatusCode}] Successfully retrieved {pagedResult.Items.Count} products.");
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