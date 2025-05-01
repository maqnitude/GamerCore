namespace GamerCore.Core.Entities
{
    public class ProductReview : BaseEntity
    {
        public int ProductReviewId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        // Add user later
    }
}