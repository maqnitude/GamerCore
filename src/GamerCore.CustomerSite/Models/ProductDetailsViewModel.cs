namespace GamerCore.CustomerSite.Models
{
    public class ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string DescriptionHtml { get; set; } = string.Empty;
        public string WarrantyHtml { get; set; } = string.Empty;

        public List<ProductImageViewModel> Images { get; set; } = [];

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductReviewViewModel> Reviews { get; set; } = [];
    }
}