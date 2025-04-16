namespace GamerCore.CustomerSite.Models
{
    public class ProductListViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; } = [];
        public PaginationMetadata Pagination { get; set; } = null!;
    }
}