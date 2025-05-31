using GamerCore.Core.Entities;

namespace GamerCore.Api.Tests.Utilities
{
    public static class ProductGenerator
    {
        private static List<Product> _products = [];
        private static List<Category> _categories = [];

        public static List<Product> Products => _products;
        public static List<Category> Categories => _categories;

        public static List<Product> Generate(
            int productCount = 20,
            int categoryCount = 10,
            int imageCount = 5,
            int reviewCount = 10)
        {
            // Reset every time this method is called
            _products = [];
            _categories = CategoryGenerator.Generate(categoryCount);
            var random = new Random();

            foreach (var category in _categories)
            {
                for (int i = 1; i <= productCount; i++)
                {
                    var productId = Guid.NewGuid();

                    var product = new Product
                    {
                        Id = productId,
                        Name = $"Product {productId}",
                        Price = random.Next(1, 1000) * 10.00M,
                    };

                    var productCategory = new ProductCategory
                    {
                        ProductId = productId,
                        Product = product,
                        CategoryId = category.Id,
                        Category = category
                    };

                    product.ProductCategories.Add(productCategory);

                    var productDetail = new ProductDetail
                    {
                        Id = Guid.NewGuid(),
                        DescriptionHtml = $"<p>Detailed description for Product {productId}</p><ul><li>Feature 1</li><li>Feature 2</li></ul>",
                        WarrantyHtml = $"<p>Warranty information for Product {productId}</p><p>1 year limited warranty</p>",
                        ProductId = productId,
                        Product = product
                    };

                    product.Detail = productDetail;

                    for (int j = 0; j < imageCount; j++)
                    {
                        var productImage = new ProductImage
                        {
                            Id = Guid.NewGuid(),
                            Url = "https://placehold.co/600x800",
                            IsPrimary = j == 0,
                            ProductId = productId,
                            Product = product
                        };

                        product.Images.Add(productImage);
                    }

                    for (int j = 0; j < reviewCount; j++)
                    {
                        var productReview = new ProductReview
                        {
                            Id = Guid.NewGuid(),
                            Rating = 3,
                            ReviewTitle = "Review title",
                            ReviewText = "Review text",
                            ProductId = productId,
                            Product = product
                        };

                        product.Reviews.Add(productReview);
                    }

                    _products.Add(product);
                }
            }

            return _products;
        }
    }
}