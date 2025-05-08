using GamerCore.Api.Models;
using GamerCore.Core.Models;

namespace GamerCore.Api.Services
{
    public interface IReviewService
    {
        /// <summary>
        /// Create a new product review.
        /// </summary>
        /// <param name="createReviewDto"></param>
        /// <returns>The created product review DTO. Null if user already reviewed the product.</returns>
        Task<ProductReviewDto?> CreateReviewAsync(CreateReviewDto createReviewDto);
    }
}