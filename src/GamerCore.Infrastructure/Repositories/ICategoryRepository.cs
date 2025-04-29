using GamerCore.Core.Entities;

namespace GamerCore.Infrastructure.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetQueryableCategories();
        void AddCategory(Category category);
        void RemoveCategory(Category category);
    }
}