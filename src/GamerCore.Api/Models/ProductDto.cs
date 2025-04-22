namespace GamerCore.Api.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; } = [];
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}