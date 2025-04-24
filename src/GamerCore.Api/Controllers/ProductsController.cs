using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService service, ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProductsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? categoryIds = null)
        {
            try
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

                var result = await _service.GetFilteredProductsAsync(page, pageSize, categoryIdArray);

                if (result.Items.Count == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult<ProductDetailsDto>> GetProductDetailsAsync(int id)
        {
            try
            {
                var productDetailsDto = await _service.GetProductDetailsAsync(id);

                if (productDetailsDto == null)
                {
                    return NotFound();
                }

                return Ok(productDetailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product details.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}