using GamerCore.Api.Models;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("page/{page:int}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsAsync(
            int page = 1,
            int? pageSize = null,
            [FromQuery] int[]? categoryIds = null)
        {
            return await GetFilteredProductsAsync(page, pageSize, categoryIds);
        }

        private async Task<ActionResult<List<ProductDto>>> GetFilteredProductsAsync(
            int page,
            int? pageSize = null,
            int[]? categoryIds = null)
        {
            try
            {
                int effectivePage = page;
                effectivePage = Math.Max(effectivePage, 1);

                int effectivePageSize = pageSize ?? PaginationConstants.DefaultPageSize;
                effectivePageSize = Math.Min(effectivePageSize, PaginationConstants.MaxPageSize);
                effectivePageSize = Math.Max(effectivePageSize, 1);

                var queryableProducts = _repository.GetQueryableProducts();

                // Filter by categories
                if (categoryIds != null && categoryIds.Length > 0)
                {
                    // Products must have all the specified categories
                    queryableProducts = queryableProducts
                        .Where(p => categoryIds
                            .All(categoryId => p.ProductCategories
                                .Any(pc => pc.CategoryId == categoryId)));
                }

                if (!await queryableProducts.AnyAsync())
                {
                    _logger.LogWarning("No products found.");
                    return NoContent();
                }

                // This is for pagination on client-side
                var totalProducts = await queryableProducts.CountAsync();

                var productDtos = await queryableProducts
                    .Skip((effectivePage - 1) * effectivePageSize)
                    .Take(effectivePageSize)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Categories = p.ProductCategories.Select(pc => new CategoryDto
                        {
                            CategoryId = pc.Category.CategoryId,
                            Name = pc.Category.Name
                        })
                    }).ToListAsync();

                var pagedResult = new PagedResult<ProductDto>
                {
                    Items = productDtos,
                    Page = effectivePage,
                    PageSize = effectivePageSize,
                    TotalItems = totalProducts
                };

                _logger.LogInformation($"Retrieved {productDtos.Count} products.");
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}