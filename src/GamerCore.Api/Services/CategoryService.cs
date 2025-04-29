using GamerCore.Api.Models;
using GamerCore.Core.Entities;
using GamerCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var queryableCategories = _unitOfWork.Categories.GetQueryableCategories();

            var totalCategories = await queryableCategories.CountAsync();

            if (totalCategories == 0)
            {
                _logger.LogWarning("No category found.");
                return [];
            }

            var categoryDtos = await queryableCategories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.ProductCategories.Count()
                })
                .ToListAsync();

            _logger.LogInformation($"Successfully retrieved {categoryDtos.Count} categories.");
            return categoryDtos;
        }

        public async Task<CategoryDto?> GetCategoryAsync(int id)
        {
            var queryableCategories = _unitOfWork.Categories.GetQueryableCategories();

            var categoryDto = await queryableCategories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.ProductCategories.Count
                })
                .SingleOrDefaultAsync();

            if (categoryDto == null)
            {
                return null;
            }

            return categoryDto;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };

            // Add and track
            _unitOfWork.Categories.AddCategory(category);

            // Commit change
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await FindCategoryByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            _unitOfWork.Categories.RemoveCategory(category);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<Category?> FindCategoryByIdAsync(int id)
        {
            var queryableCategories = _unitOfWork.Categories.GetQueryableCategories();

            var category = await queryableCategories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return null;
            }

            return category;
        }
    }
}