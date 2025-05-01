namespace GamerCore.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public int ProductImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}