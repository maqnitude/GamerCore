namespace GamerCore.Core.Entities
{
    public class ProductDetail : BaseEntity
    {
        public int ProductDetailId { get; set; }
        public string DescriptionHtml { get; set; } = string.Empty;
        public string WarrantyHtml { get; set; } = string.Empty;

        // Foreign key and reference
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}