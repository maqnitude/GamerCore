using GamerCore.Core.Entities;

namespace GamerCore.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogContext _context;

        public CategoryRepository(CatalogContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetQueryableCategories()
        {
            return _context.Categories
                .OrderBy(c => c.Name);
        }
    }
}