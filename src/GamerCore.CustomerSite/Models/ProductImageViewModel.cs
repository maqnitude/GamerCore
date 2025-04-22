namespace GamerCore.CustomerSite.Models
{
    public class ProductImageViewModel
    {
        public int ProductImageId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}