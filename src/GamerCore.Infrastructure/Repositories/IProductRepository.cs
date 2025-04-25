using GamerCore.Core.Entities;

namespace GamerCore.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetQueryableProducts();
        void AddProduct(Product product);
    }
}