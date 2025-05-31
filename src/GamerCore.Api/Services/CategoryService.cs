using System.Diagnostics;
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
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.ProductCategories.Count(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Successfully retrieved {Count} categories.", categoryDtos.Count);
            return categoryDtos;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(string id)
        {
            var queryableCategories = _unitOfWork.Categories.GetQueryableCategories();

            var parsedId = Guid.Parse(id);

            var categoryDto = await queryableCategories
                .AsNoTracking()
                .Where(c => c.Id == parsedId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.ProductCategories.Count,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
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
                Id = category.Id.ToString(),
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(string id, UpdateCategoryDto updateCategoryDto)
        {
            Debug.Assert(id.Equals(updateCategoryDto.Id, StringComparison.OrdinalIgnoreCase));

            var category = await FindCategoryByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            // Update and track if not already tracked
            _unitOfWork.Categories.UpdateCategory(category);

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;

            // Commit
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        public async Task<bool> DeleteCategoryAsync(string id)
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

        private async Task<Category?> FindCategoryByIdAsync(string id)
        {
            var queryableCategories = _unitOfWork.Categories.GetQueryableCategories();

            var parsedId = Guid.Parse(id);

            var category = await queryableCategories
                .FirstOrDefaultAsync(c => c.Id == parsedId);

            if (category == null)
            {
                return null;
            }

            return category;
        }
    }
}