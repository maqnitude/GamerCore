using System.Security.Claims;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.CustomerSite.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [Authorize(Roles = RoleConstants.Customer)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateReviewViewModel createReviewViewModel)
        {
            // Redirect to /Home/Error for all errors for now
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            try
            {
                // TODO: Fix this after integrating OpenIddict
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Error", "Home");
                }

                var createReviewDto = new CreateReviewDto
                {
                    Rating = createReviewViewModel.Rating,
                    ReviewTitle = createReviewViewModel.ReviewTitle,
                    ReviewText = createReviewViewModel.ReviewText,
                    ProductId = createReviewViewModel.ProductId,
                    // UserId will be set in here until OpenIddict authorization server is implemented
                    UserId = userId
                };

                var result = await _reviewService.CreateReviewAsync(createReviewDto);

                if (result)
                {
                    return RedirectToAction("Details", "Products", new { id = createReviewDto.ProductId });
                }

                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product review");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}