using System.Security.Claims;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [Authorize(
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
            Roles = RoleNames.User)]
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreateReviewAsync(CreateReviewViewModel createReviewViewModel)
        {
            // Redirect to /Home/Error for all errors for now
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            try
            {
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
                    UserId = userId,
                    ProductId = createReviewViewModel.ProductId
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