namespace GamerCore.Core.Entities
{
    // Use singlar in favor of EF Core convention
    public class ProductDetail : BaseEntity
    {
        public Guid Id { get; set; }

        public string DescriptionHtml { get; set; } = string.Empty;

        public string WarrantyHtml { get; set; } = string.Empty;

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}