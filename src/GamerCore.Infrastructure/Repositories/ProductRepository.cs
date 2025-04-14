using GamerCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogContext _context;

        public ProductRepository(CatalogContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetProducts()
        {
            return _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category);
        }
    }
}