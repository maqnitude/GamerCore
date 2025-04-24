using GamerCore.Api.Models;
using GamerCore.Core.Models;

namespace GamerCore.Api.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetFilteredProductsAsync(int page, int? pageSize, int[]? categoryIds);
        Task<ProductDetailsDto?> GetProductDetailsAsync(int id);
    }
}