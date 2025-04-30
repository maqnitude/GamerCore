using GamerCore.Api.Models;
using GamerCore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService service, ILogger<CategoriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet(Name = "GetCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CategoryDto>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _service.GetCategoriesAsync();

                if (categories == null || categories.Count == 0)
                {
                    return NoContent();
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var categoryDto = await _service.GetCategoryAsync(id);

                if (categoryDto == null)
                {
                    return NotFound();
                }

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCategoryDto = await _service.CreateCategoryAsync(createCategoryDto);

                return CreatedAtAction(
                    "GetCategoryById",
                    "Categories",
                    new { id = createdCategoryDto.CategoryId },
                    createdCategoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating category.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCategoryDto = await _service.UpdateCategoryAsync(id, updateCategoryDto);

                if (updatedCategoryDto == null)
                {
                    return NotFound();
                }

                return Ok(updatedCategoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating category.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                var result = await _service.DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting category.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}