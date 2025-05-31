using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Data;

namespace GamerCore.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProductReview> GetQueryableReviews()
        {
            return _context.ProductReviews;
        }

        public void AddReview(ProductReview productReview)
        {
            _context.ProductReviews.Add(productReview);
        }
    }
}