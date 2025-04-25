using System.ComponentModel.DataAnnotations;

namespace GamerCore.Api.Models
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        public string DescriptionHtml { get; set; } = string.Empty;

        public string WarrantyHtml { get; set; } = string.Empty;

        public List<int> CategoryIds { get; set; } = [];

        [Required]
        [Url]
        public string PrimaryImageUrl { get; set; } = string.Empty;

        public List<string>? ImageUrls { get; set; }
    }
}