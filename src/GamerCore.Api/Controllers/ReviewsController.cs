using GamerCore.Api.Services;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult GetReviewById(string id)
        {
            // Does nothing right now
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // TODO: Fix this after integrating OpenIddict
                // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // if (string.IsNullOrEmpty(userId))
                // {
                //     return Unauthorized("No authenticated user found.");
                // }

                // Set the user id string here and not in client-side
                // createReviewDto.UserId = userId;

                var productReviewDto = await _reviewService.CreateReviewAsync(createReviewDto);
                if (productReviewDto == null)
                {
                    _logger.LogError("User has already reviewed this product.");
                    return Conflict();
                }

                return CreatedAtAction(
                    nameof(GetReviewById),
                    "Reviews",
                    new { id = productReviewDto.Id },
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