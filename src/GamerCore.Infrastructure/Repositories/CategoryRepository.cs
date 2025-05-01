using GamerCore.Core.Entities;

namespace GamerCore.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CatalogDbContext _context;

        public CategoryRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetQueryableCategories()
        {
            return _context.Categories.AsQueryable();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
        }

        public void RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}