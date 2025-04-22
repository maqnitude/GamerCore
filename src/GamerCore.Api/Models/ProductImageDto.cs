namespace GamerCore.Api.Models
{
    public class ProductImageDto
    {
        public int ProductImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}