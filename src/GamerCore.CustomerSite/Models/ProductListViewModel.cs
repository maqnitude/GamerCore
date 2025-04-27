namespace GamerCore.CustomerSite.Models
{
    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; } = [];
        public ProductFilterViewModel Filter { get; set; } = null!;
        public PaginationMetadata Pagination { get; set; } = null!;
    }
}