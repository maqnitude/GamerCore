namespace GamerCore.Api.Models
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsFeatured { get; set; }

        public IEnumerable<CategoryDto> Categories { get; set; } = [];

        public string ThumbnailUrl { get; set; } = string.Empty;

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}