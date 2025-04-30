using System.Diagnostics;
using GamerCore.Api.Models;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using GamerCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GamerCore.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedList<ProductDto>> GetFilteredProductsAsync(int page, int? pageSize, int[]? categoryIds)
        {
            int effectivePage = page;
            effectivePage = Math.Max(effectivePage, 1);

            int effectivePageSize = pageSize ?? PaginationConstants.DefaultPageSize;
            effectivePageSize = Math.Min(effectivePageSize, PaginationConstants.MaxPageSize);
            effectivePageSize = Math.Max(effectivePageSize, 1);

            var queryableProducts = _unitOfWork.Products.GetQueryableProducts();

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
                return new PaginatedList<ProductDto>();
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

            return new PaginatedList<ProductDto>
            {
                Items = productDtos,
                Page = effectivePage,
                PageSize = effectivePageSize,
                TotalItems = totalProducts
            };
        }

        public async Task<ProductDetailsDto?> GetProductDetailsAsync(int id)
        {
            var queryableProducts = _unitOfWork.Products.GetQueryableProducts();

            var productDetailsDto = await queryableProducts
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDetailsDto
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
                    DescriptionHtml = p.Detail.DescriptionHtml,
                    WarrantyHtml = p.Detail.WarrantyHtml,
                    Images = p.Images
                        .Select(i => new ProductImageDto
                        {
                            ProductImageId = i.ProductImageId,
                            Url = i.Url,
                            IsPrimary = i.IsPrimary
                        }),
                    AverageRating = p.Reviews.Count != 0
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

        public async Task<int> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price
            };

            // Add product and begin tracking
            _unitOfWork.Products.AddProduct(product);

            // Add categories
            if (createProductDto.CategoryIds != null && createProductDto.CategoryIds.Count != 0)
            {
                foreach (var categoryId in createProductDto.CategoryIds)
                {
                    product.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = categoryId
                    });
                }
            }

            // Add detail
            product.Detail = new ProductDetail
            {
                DescriptionHtml = createProductDto.DescriptionHtml,
                WarrantyHtml = createProductDto.WarrantyHtml,
                Product = product
            };

            // Add primary image
            product.Images.Add(new ProductImage
            {
                Url = createProductDto.PrimaryImageUrl,
                IsPrimary = true,
                Product = product
            });

            // Add the other images
            if (createProductDto.ImageUrls != null && createProductDto.ImageUrls.Count > 0)
            {
                foreach (var imageUrl in createProductDto.ImageUrls)
                {
                    product.Images.Add(new ProductImage
                    {
                        Url = imageUrl,
                        IsPrimary = false,
                        Product = product
                    });
                }
            }

            // Commit changes
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product created successfully (id: {Id})", product.ProductId);
            return product.ProductId;
        }

        public async Task<int?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            Debug.Assert(id == updateProductDto.ProductId);

            var product = await FindProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product not found. (id: {Id})", id);
                return null;
            }

            // Update product and begin tracking (if not already tracked after being found)
            _unitOfWork.Products.UpdateProduct(product);

            product.Name = updateProductDto.Name;
            product.Price = updateProductDto.Price;

            // Replace categories
            product.ProductCategories.Clear();

            foreach (var categoryId in updateProductDto.CategoryIds)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    ProductId = id,
                    CategoryId = categoryId
                });
            }

            // Replace details
            product.Detail = new ProductDetail
            {
                DescriptionHtml = updateProductDto.DescriptionHtml,
                WarrantyHtml = updateProductDto.WarrantyHtml
            };

            // Replace image urls
            product.Images.Clear();

            product.Images.Add(new ProductImage
            {
                Url = updateProductDto.PrimaryImageUrl,
                IsPrimary = true,
                ProductId = id
            });

            if (updateProductDto.ImageUrls != null && updateProductDto.ImageUrls.Count > 0)
            {
                foreach (var imageUrl in updateProductDto.ImageUrls)
                {
                    product.Images.Add(new ProductImage
                    {
                        Url = imageUrl,
                        IsPrimary = false,
                        ProductId = id
                    });
                }
            }

            // Commit changes
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product updated successfully (id: {Id})", id);
            return id;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await FindProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Failed to delete. Product not found. (id: {Id})", id);
                return false;
            }

            _unitOfWork.Products.RemoveProduct(product);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product deleted successfully (id: {Id})", id);
            return true;
        }

        private async Task<Product?> FindProductByIdAsync(int id)
        {
            var queryableProducts = _unitOfWork.Products.GetQueryableProducts();

            var product = await queryableProducts
                .Include(p => p.ProductCategories)
                .Include(p => p.Detail)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return null;
            }

            return product;
        }
    }
}