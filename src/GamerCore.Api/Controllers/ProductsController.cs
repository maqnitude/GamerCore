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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginatedList<ProductDto>>> GetProductsAsync(
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

        [HttpGet("{id}", Name = "GetProductDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdProductId = await _service.CreateProductAsync(createProductDto);

                return CreatedAtAction(
                    "GetProductDetails",
                    "Products",
                    new { id = createdProductId },
                    createdProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedProductId = await _service.UpdateProductAsync(id, updateProductDto);

                if (updatedProductId == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                var result = await _service.DeleteProductAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product (id: {Id}).", id);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}