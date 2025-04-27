using GamerCore.Api.Models;
using GamerCore.Core.Models;

namespace GamerCore.Api.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetFilteredProductsAsync(int page, int? pageSize, int[]? categoryIds);
        Task<ProductDetailsDto?> GetProductDetailsAsync(int id);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <returns>Created product's ID.</returns>
        Task<int> CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> DeleteProductAsync(int id);
    }
}