using GamerCore.Api.Models;

namespace GamerCore.Api.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> GetCategoryAsync(int id);

        /// <summary>
        /// Create a category.
        /// </summary>
        /// <param name="createCategoryDto"></param>
        /// <returns>The created category.</returns>
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        /// <summary>
        /// Find and delete a category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if found and deleted. False if not found.</returns>
        Task<bool> DeleteCategoryAsync(int id);
    }
}