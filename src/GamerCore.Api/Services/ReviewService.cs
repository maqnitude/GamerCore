using GamerCore.Api.Models;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using GamerCore.Infrastructure.Repositories;

namespace GamerCore.Api.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductReviewDto?> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var reviewExisted = _unitOfWork.Reviews.GetQueryableReviews()
                .Where(r => r.UserId == createReviewDto.UserId && r.ProductId == createReviewDto.ProductId)
                .Any();

            if (reviewExisted)
            {
                return null;
            }

            var productReview = new ProductReview
            {
                Rating = createReviewDto.Rating,
                ReviewTitle = createReviewDto.ReviewTitle,
                ReviewText = createReviewDto.ReviewText,
                UserId = createReviewDto.UserId,
                ProductId = createReviewDto.ProductId
            };

            // Add and track
            _unitOfWork.Reviews.AddReview(productReview);

            // Commit
            await _unitOfWork.SaveChangesAsync();

            return new ProductReviewDto
            {
                ProductReviewId = productReview.ProductReviewId,
                Rating = productReview.Rating,
                ReviewTitle = productReview.ReviewTitle,
                ReviewText = productReview.ReviewText,
                UserId = productReview.UserId
            };
        }
    }
}