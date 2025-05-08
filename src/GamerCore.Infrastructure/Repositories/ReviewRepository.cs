using GamerCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CatalogDbContext _context;

        public ReviewRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public IQueryable<ProductReview> GetQueryableReviews()
        {
            return _context.ProductReviews
                .Include(r => r.Product);
        }

        public void AddReview(ProductReview productReview)
        {
            _context.ProductReviews.Add(productReview);
        }
    }
}