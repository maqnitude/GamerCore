namespace GamerCore.CustomerSite.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Need this to display the product count pill on each category.
        public int ProductCount { get; set; }
    }
}