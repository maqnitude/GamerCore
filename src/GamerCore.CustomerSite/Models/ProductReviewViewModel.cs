namespace GamerCore.CustomerSite.Models
{
    public class ProductReviewViewModel
    {
        public string Id { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? ReviewTitle { get; set; }

        public string? ReviewText { get; set; }

        public string UserFirstName { get; set; } = string.Empty;

        public string UserLastName { get; set; } = string.Empty;
    }
}