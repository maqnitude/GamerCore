using GamerCore.Core.Entities;

namespace GamerCore.Infrastructure.Repositories
{
    public interface IReviewRepository
    {
        IQueryable<ProductReview> GetQueryableReviews();
        void AddReview(ProductReview productReview);
    }
}