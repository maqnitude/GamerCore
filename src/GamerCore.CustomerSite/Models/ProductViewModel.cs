namespace GamerCore.CustomerSite.Models
{
    public class ProductViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsFeatured { get; set; }

        public List<CategoryViewModel> Categories { get; set; } = [];

        public string ThumbnailUrl { get; set; } = string.Empty;

        public double AverageRating { get; set; }

        public int ReviewCount { get; set; }
    }
}