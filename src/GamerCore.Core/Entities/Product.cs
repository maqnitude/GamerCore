using System.ComponentModel.DataAnnotations.Schema;

namespace GamerCore.Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; } = [];
    }
}