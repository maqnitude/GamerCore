namespace GamerCore.Api.Models
{
    public class ProductImageDto
    {
        public string Id { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}