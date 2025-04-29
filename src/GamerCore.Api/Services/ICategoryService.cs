using GamerCore.Api.Models;

namespace GamerCore.Api.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryAsync(int id);
        Task<int> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    }
}