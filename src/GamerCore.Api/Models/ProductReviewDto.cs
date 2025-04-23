namespace GamerCore.Api.Models
{
    public class ProductReviewDto
    {
        public int ProductReviewId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
    }
}