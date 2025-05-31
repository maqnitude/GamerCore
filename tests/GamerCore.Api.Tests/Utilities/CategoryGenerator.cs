using GamerCore.Core.Entities;

namespace GamerCore.Api.Tests.Utilities
{
    public static class CategoryGenerator
    {
        private static List<Category> _categories = [];

        public static List<Category> Categories => _categories;

        public static List<Category> Generate(int categoryCount = 10)
        {
            // Reset
            _categories = [];

            for (int i = 1; i <= categoryCount; i++)
            {
                var id = Guid.NewGuid();
                var category = new Category
                {
                    Id = id,
                    Name = $"Category {id}",
                    Description = $"Category description {id}"
                };

                _categories.Add(category);
            }

            return _categories;
        }
    }
}