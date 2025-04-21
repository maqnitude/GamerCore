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
        public async Task<ActionResult<List<ProductDto>>> GetProductsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? categoryIds = null)
        {
            int[]? categoryIdArray = null;

            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                categoryIdArray = categoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out int parsedId) ? parsedId : (int?)null)
                    .Where(id => id != null)
                    .Select(id => id!.Value)
                    .ToArray();
            }

            return await GetFilteredProductsAsync(page, pageSize, categoryIdArray);
        }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult<ProductDetailsDto>> GetProductDetailsAsync(int id)
        {
            var queryableProducts = _repository.GetQueryableProducts();

            var productDetailsDto = await queryableProducts
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDetailsDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    DescriptionHtml = p.Detail.DescriptionHtml,
                    WarrantyHtml = p.Detail.WarrantyHtml
                })
                .SingleOrDefaultAsync();

            if (productDetailsDto == null)
            {
                return NotFound();
            }

            return Ok(productDetailsDto);
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
                    // Products must have any of the specified categories
                    queryableProducts = queryableProducts
                        .Where(p => p.ProductCategories
                            .Any(pc => categoryIds.Contains(pc.CategoryId)));
                }

                if (!await queryableProducts.AnyAsync())
                {
                    _logger.LogWarning("No products found.");
                    return NoContent();
                }

                // This is for pagination on client-side
                var totalProducts = await queryableProducts.CountAsync();

                var productDtos = await queryableProducts
                    .AsNoTracking()
                    .Skip((effectivePage - 1) * effectivePageSize)
                    .Take(effectivePageSize)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Categories = p.ProductCategories.Select(pc => new CategoryDto
                        {
                            CategoryId = pc.Category.CategoryId,
                            Name = pc.Category.Name
                        })
                    })
                    .ToListAsync();

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