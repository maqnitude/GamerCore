namespace GamerCore.CustomerSite.Models
{
    public class ProductFilterViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = [];

        public string? SelectedCategoryId { get; set; }

        // Add more filters here
    }
}