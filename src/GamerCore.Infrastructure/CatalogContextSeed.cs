using GamerCore.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamerCore.Infrastructure
{
    public static class CatalogContextSeed
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Products.Any())
            {
                SeedProducts(context);
            }

            if (!context.Categories.Any())
            {
                SeedCategories(context);
            }
        }

        private static void SeedProducts(CatalogContext context)
        {
            var categories = context.Categories.ToList();
            var products = new List<Product>();
            var productCategories = new List<ProductCategory>();
            var random = new Random();

            // Define price ranges for each category
            var categoryPriceRanges = new Dictionary<string, (int MinDollars, int MaxDollars)>
            {
                { "Consoles", (100, 2999) },
                { "PCs", (100, 5999) },
                { "Laptops", (100, 3999) },
                { "Controllers", (20, 199) },
                { "Headsets", (20, 199) },
                { "Monitors", (100, 999) },
                { "Keyboards", (20, 199) },
                { "Mice", (20, 199) }
            };

            // Define singular forms for consistent product naming
            var categorySingulars = new Dictionary<string, string>
            {
                { "Consoles", "Console" },
                { "PCs", "PC" },
                { "Laptops", "Laptop" },
                { "Controllers", "Controller" },
                { "Headsets", "Headset" },
                { "Monitors", "Monitor" },
                { "Keyboards", "Keyboard" },
                { "Mice", "Mouse" }
            };

            foreach (var category in categories)
            {
                var priceRange = categoryPriceRanges[category.Name];
                var singular = categorySingulars[category.Name];
                for (int i = 1; i <= 20; i++)
                {
                    int dollars = random.Next(priceRange.MinDollars, priceRange.MaxDollars + 1);
                    int cents = random.Next(0, 100);
                    decimal price = dollars + (cents / 100m);

                    var product = new Product
                    {
                        Name = $"{singular} {i}", // e.g., "Mouse 1", "Console 1"
                        Price = price
                    };
                    products.Add(product);

                    // Link the product to its category
                    productCategories.Add(new ProductCategory
                    {
                        Product = product,
                        Category = category
                    });
                }
            }

            context.Products.AddRange(products);
            context.ProductCategories.AddRange(productCategories);
            context.SaveChanges();
        }

        private static void SeedCategories(CatalogContext context)
        {
            var categories = new List<Category>
            {
                new() { Name = "Consoles" },
                new() { Name = "PCs" },
                new() { Name = "Laptops" },
                new() { Name = "Controllers" },
                new() { Name = "Headsets" },
                new() { Name = "Monitors" },
                new() { Name = "Keyboards" },
                new() { Name = "Mice" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}