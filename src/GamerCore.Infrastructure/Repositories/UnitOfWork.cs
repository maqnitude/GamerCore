namespace GamerCore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _context;

        public UnitOfWork(
            CatalogDbContext context,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IReviewRepository reviewRepository)
        {
            _context = context;
            Categories = categoryRepository;
            Products = productRepository;
            Reviews = reviewRepository;
        }

        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IReviewRepository Reviews { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}