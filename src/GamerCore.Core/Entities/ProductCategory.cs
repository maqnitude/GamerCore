namespace GamerCore.Core.Entities
{
    public class ProductCategory : BaseEntity
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}