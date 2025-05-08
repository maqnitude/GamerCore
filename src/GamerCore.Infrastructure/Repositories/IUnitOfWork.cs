namespace GamerCore.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IReviewRepository Reviews { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}