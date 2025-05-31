namespace GamerCore.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}