namespace GamerCore.Api.Models
{
    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public IEnumerable<CategoryDto> Categories { get; set; } = [];

        public string DescriptionHtml { get; set; } = string.Empty;
        public string WarrantyHtml { get; set; } = string.Empty;

        public IEnumerable<ProductImageDto> Images { get; set; } = [];

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public IEnumerable<ProductReviewDto> Reviews { get; set; } = [];
    }
}