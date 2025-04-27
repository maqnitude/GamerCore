using GamerCore.Api.Models;
using GamerCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryRepository repository, ILogger<CategoriesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategoriesAsync()
        {
            try
            {
                var queryableCategories = _repository.GetQueryableCategories();

                if (!await queryableCategories.AnyAsync())
                {
                    _logger.LogWarning("No category found.");
                    return NoContent();
                }

                var categoryDtos = await queryableCategories
                    .AsNoTracking()
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        ProductCount = c.ProductCategories.Count()
                    })
                    .ToListAsync();

                _logger.LogInformation($"Successfully retrieved {categoryDtos.Count} categories.");
                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}