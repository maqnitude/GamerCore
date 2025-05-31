namespace GamerCore.Core.Entities
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<ProductCategory> ProductCategories { get; set; } = [];
    }
}