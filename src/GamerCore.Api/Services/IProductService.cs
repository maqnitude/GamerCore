using GamerCore.Api.Models;
using GamerCore.Core.Models;

namespace GamerCore.Api.Services
{
    public interface IProductService
    {
        Task<PaginatedList<ProductDto>> GetFilteredProductsAsync(int page, int? pageSize, string[]? categoryIds);
        Task<List<ProductDto>> GetFeaturedProductsAsync();
        Task<ProductDetailsDto?> GetProductDetailsAsync(string id);

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <returns>The created product's ID.</returns>
        Task<string> CreateProductAsync(CreateProductDto createProductDto);

        /// <summary>
        /// Find and update an existing product.
        /// </summary>
        /// <param name="updateProductDto"></param>
        /// <returns>The updated product's ID. Null if not found.</returns>
        Task<string?> UpdateProductAsync(string id, UpdateProductDto updateProductDto);

        /// <summary>
        /// Find and delete an existing product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the product is found and deleted. False if the product is not found.
        /// </returns>
        Task<bool> DeleteProductAsync(string id);
    }
}