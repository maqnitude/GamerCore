using GamerCore.Api.Models;
using GamerCore.Api.Services;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
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
        public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null,
            [FromQuery] string? categoryIds = null)
        {
            try
            {
                string[]? categoryIdArray = null;

                if (!string.IsNullOrWhiteSpace(categoryIds))
                {
                    categoryIdArray = [.. categoryIds
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Where(id => id != null)
                        .Select(id => id!)];
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

        [HttpGet("featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts()
        {
            try
            {
                var result = await _service.GetFeaturedProductsAsync();

                if (result.Count == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving featured products.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(string id)
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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdProductId = await _service.CreateProductAsync(createProductDto);

                return CreatedAtAction(
                    nameof(GetProductDetails),
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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(string id)
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