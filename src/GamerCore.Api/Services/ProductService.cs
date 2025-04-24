using GamerCore.Api.Models;
using GamerCore.Core.Constants;
using GamerCore.Core.Models;
using GamerCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResult<ProductDto>> GetFilteredProductsAsync(int page, int? pageSize, int[]? categoryIds)
        {
            int effectivePage = page;
            effectivePage = Math.Max(effectivePage, 1);

            int effectivePageSize = pageSize ?? PaginationConstants.DefaultPageSize;
            effectivePageSize = Math.Min(effectivePageSize, PaginationConstants.MaxPageSize);
            effectivePageSize = Math.Max(effectivePageSize, 1);

            var queryableProducts = _repository.GetQueryableProducts();

            // Filter by categories
            if (categoryIds != null && categoryIds.Length > 0)
            {
                // Products must have any of the specified categories
                queryableProducts = queryableProducts
                    .Where(p => p.ProductCategories
                        .Any(pc => categoryIds.Contains(pc.CategoryId)));
            }

            // This is for pagination on client-side
            var totalProducts = await queryableProducts.CountAsync();

            if (totalProducts == 0)
            {
                _logger.LogWarning("No products found.");
                return new PagedResult<ProductDto>();
            }

            var productDtos = await queryableProducts
                .AsNoTracking()
                .Skip((effectivePage - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Categories = p.ProductCategories
                        .Select(pc => new CategoryDto
                        {
                            CategoryId = pc.Category.CategoryId,
                            Name = pc.Category.Name
                        }),
                    ThumbnailUrl = p.Images
                        .Where(i => i.IsPrimary == true)
                        .Select(i => i.Url)
                        .First(),
                    AverageRating = p.Reviews.Count != 0
                        ? p.Reviews.Average(r => r.Rating)
                        : 0.0,
                    ReviewCount = p.Reviews.Count()
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} products.", productDtos.Count);

            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                Page = effectivePage,
                PageSize = effectivePageSize,
                TotalItems = totalProducts
            };
        }

        public async Task<ProductDetailsDto?> GetProductDetailsAsync(int id)
        {
            var queryableProducts = _repository.GetQueryableProducts();

            var productDetailsDto = await queryableProducts
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDetailsDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    DescriptionHtml = p.Detail.DescriptionHtml,
                    WarrantyHtml = p.Detail.WarrantyHtml,
                    Images = p.Images
                        .Select(i => new ProductImageDto
                        {
                            ProductImageId = i.ProductImageId,
                            Url = i.Url,
                            IsPrimary = i.IsPrimary
                        }),
                    AverageRating = p.Reviews.Any()
                        ? p.Reviews.Average(r => r.Rating)
                        : 0.0,
                    ReviewCount = p.Reviews.Count(),
                    Reviews = p.Reviews.Select(r => new ProductReviewDto
                    {
                        ProductReviewId = r.ProductReviewId,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText
                    })
                })
                .SingleOrDefaultAsync();

            if (productDetailsDto == null)
            {
                _logger.LogWarning("Product not found (id: {Id}).", id);
            }
            else
            {
                _logger.LogInformation("Successfully retrieved product details (id: {Id}).", id);
            }

            return productDetailsDto;
        }
    }
}