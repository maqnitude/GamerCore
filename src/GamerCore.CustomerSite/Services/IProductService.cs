using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductViewModel>> GetProductsAsync(int page = 1, int[]? categoryIds = null);
    }
}