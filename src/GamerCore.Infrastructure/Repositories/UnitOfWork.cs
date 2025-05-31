using GamerCore.Infrastructure.Data;

namespace GamerCore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(
            ApplicationDbContext dbContext,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IReviewRepository reviewRepository)
        {
            _dbContext = dbContext;
            Categories = categoryRepository;
            Products = productRepository;
            Reviews = reviewRepository;
        }

        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IReviewRepository Reviews { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}