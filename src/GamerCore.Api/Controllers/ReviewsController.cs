using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetReviewById")]
        public IActionResult GetReviewById(int id)
        {
            // Does nothing right now
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReviewAsync([FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var productReviewDto = await _reviewService.CreateReviewAsync(createReviewDto);

                if (productReviewDto == null)
                {
                    _logger.LogError("User has already reviewed this product.");
                    return Conflict();
                }

                return CreatedAtAction(
                    "GetReviewById",
                    "Reviews",
                    new { id = productReviewDto.ProductReviewId },
                    productReviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product review.");
                return StatusCode(500, "Internal server errorl");
            }
        }
    }
}