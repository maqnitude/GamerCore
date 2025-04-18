namespace GamerCore.CustomerSite.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = [];
    }
}