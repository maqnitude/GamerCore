using GamerCore.Core.Entities;

namespace GamerCore.Api.Tests.Utilities
{
    public static class ProductGenerator
    {
        private static List<Product> _products = [];
        private static List<Category> _categories = [];

        public static List<Product> Products => _products;
        public static List<Category> Categories => _categories;

        public static List<Product> Generate(int productCount = 20, int categoryCount = 10)
        {
            // Reset every time this method is called
            _products = [];
            _categories = CategoryGenerator.Generate(categoryCount);

            var random = new Random();
            foreach (var category in _categories)
            {
                for (int i = 1; i <= productCount; i++)
                {
                    var product = new Product
                    {
                        ProductId = i,
                        Name = $"Product {i}",
                        Price = random.Next(1, 1000) * 10.00M
                    };

                    var productCategory = new ProductCategory
                    {
                        ProductId = i,
                        Product = product,
                        CategoryId = category.CategoryId,
                        Category = category
                    };

                    product.ProductCategories.Add(productCategory);

                    _products.Add(product);
                }
            }

            return _products;
        }
    }
}