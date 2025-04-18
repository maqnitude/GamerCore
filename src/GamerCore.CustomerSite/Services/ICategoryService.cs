using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetCategoriesAsync();
    }
}