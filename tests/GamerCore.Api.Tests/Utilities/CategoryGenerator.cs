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
                var category = new Category
                {
                    CategoryId = i,
                    Name = $"Category {i}",
                    Description = $"Category description {i}"
                };

                _categories.Add(category);
            }

            return _categories;
        }
    }
}