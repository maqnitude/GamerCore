namespace GamerCore.Core.Entities
{
    public class ProductReview : BaseEntity
    {
        public Guid Id { get; set; }

        public int Rating { get; set; }

        public string? ReviewTitle { get; set; }

        public string? ReviewText { get; set; }

        public string UserId { get; set; } = string.Empty;

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}