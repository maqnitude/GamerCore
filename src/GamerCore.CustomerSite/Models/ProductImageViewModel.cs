namespace GamerCore.CustomerSite.Models
{
    public class ProductImageViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}