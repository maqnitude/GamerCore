using System.ComponentModel.DataAnnotations.Schema;

namespace GamerCore.Core.Entities
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public bool IsFeatured { get; set; }

        // 1-1 navigation properties
        public ProductDetail Detail { get; set; } = null!;

        // 1-N navigation properties
        public ICollection<ProductCategory> ProductCategories { get; set; } = [];
        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<ProductReview> Reviews { get; set; } = [];
    }
}