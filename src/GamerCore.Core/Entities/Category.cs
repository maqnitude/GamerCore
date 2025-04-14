namespace GamerCore.Core.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<ProductCategory> ProductCategories { get; set; } = [];
    }
}