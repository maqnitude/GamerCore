using GamerCore.Core.Models;
using GamerCore.CustomerSite.Models;

namespace GamerCore.CustomerSite.Services
{
    public interface IProductService
    {
        Task<PaginatedList<ProductViewModel>> GetProductsAsync(int page = 1, string[]? categoryIds = null);
        Task<List<ProductViewModel>> GetFeaturedProductsAsync();
        Task<ProductDetailsViewModel> GetProductDetailsAsync(string id);
    }
}