namespace GamerCore.Api.Models
{
    public class ProductReviewDto
    {
        public string Id { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? ReviewTitle { get; set; }

        public string? ReviewText { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string UserFirstName { get; set; } = string.Empty;

        public string UserLastName { get; set; } = string.Empty;
    }
}