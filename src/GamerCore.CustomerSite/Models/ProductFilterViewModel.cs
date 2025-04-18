namespace GamerCore.CustomerSite.Models
{
    public class ProductFilterViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = [];
        public int? SelectedCategoryId { get; set; }
        // Add more filters here
    }
}