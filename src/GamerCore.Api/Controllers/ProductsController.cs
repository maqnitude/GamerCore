using GamerCore.Api.Models;
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
        public async Task<ActionResult<List<ProductDto>>> GetProductsAsync()
        {
            try
            {
                var queryableProducts = _repository.GetQueryableProducts();

                if (!await queryableProducts.AnyAsync())
                {
                    _logger.LogWarning("No products found.");
                    return NoContent();
                }

                var productDtos = await queryableProducts
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
                        }).ToList()
                    }).ToListAsync();

                _logger.LogInformation($"Retrieved {productDtos.Count} products.");
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}