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
            CatalogContext context = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<CatalogContext>();

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

            if (!context.ProductCategories.Any())
            {
                SeedProductCategories(context);
            }
        }

        private static void SeedProducts(CatalogContext context)
        {
            context.Products.AddRange(
                // Console
                new Product
                {
                    Name = "PlayStation 5",
                    Description = "Sony's next-generation gaming console featuring ultra-high-speed SSD, 4K gaming, and haptic feedback controllers.",
                    Price = 499.99M
                },
                new Product
                {
                    Name = "Xbox Series X",
                    Description = "Microsoft's most powerful console ever with true 4K gaming, up to 120fps, and quick resume feature.",
                    Price = 499.99M
                },
                new Product
                {
                    Name = "Nintendo Switch OLED",
                    Description = "Enhanced handheld gaming experience with vibrant 7-inch OLED screen and improved audio.",
                    Price = 349.99M
                },

                // PC
                new Product
                {
                    Name = "Titan X Gaming Desktop",
                    Description = "High-end gaming PC with RTX 4080, Intel i9 processor, 32GB RAM, and 2TB SSD.",
                    Price = 2499.99M
                },
                new Product
                {
                    Name = "Challenger Gaming PC",
                    Description = "Mid-range gaming desktop with RTX 4060, AMD Ryzen 7, 16GB RAM, and 1TB SSD.",
                    Price = 1299.99M
                },

                // Laptop
                new Product
                {
                    Name = "ROG Strix Gaming Laptop",
                    Description = "17.3-inch gaming laptop with RTX 4070, Intel i7, 16GB RAM, and 1TB SSD.",
                    Price = 1799.99M
                },
                new Product
                {
                    Name = "Razer Blade 15",
                    Description = "Ultra-thin gaming laptop with RTX 4060, Intel i7, 16GB RAM, and QHD 240Hz display.",
                    Price = 1999.99M
                },

                // Controller
                new Product
                {
                    Name = "Xbox Elite Controller Series 2",
                    Description = "Premium controller with adjustable-tension thumbsticks, wrap-around rubberized grip, and up to 40 hours of battery life.",
                    Price = 179.99M
                },
                new Product
                {
                    Name = "DualSense Wireless Controller",
                    Description = "PlayStation 5 controller featuring haptic feedback and adaptive triggers for immersive gaming.",
                    Price = 69.99M
                },

                // Headset
                new Product
                {
                    Name = "SteelSeries Arctis Nova Pro",
                    Description = "High-fidelity gaming headset with active noise cancellation and hot-swappable batteries.",
                    Price = 349.99M
                },
                new Product
                {
                    Name = "HyperX Cloud Alpha",
                    Description = "Gaming headset with DTS spatial audio and durable aluminum frame.",
                    Price = 99.99M
                },

                // Accessory
                new Product
                {
                    Name = "Elgato Stream Deck",
                    Description = "15-key customizable control pad for streamers with LCD keys.",
                    Price = 149.99M
                },
                new Product
                {
                    Name = "WD_BLACK 2TB External SSD",
                    Description = "Portable gaming SSD with up to 2000MB/s read speeds, compatible with consoles and PC.",
                    Price = 229.99M
                },

                // VR
                new Product
                {
                    Name = "Meta Quest 3",
                    Description = "All-in-one VR headset with mixed reality capabilities and high-resolution display.",
                    Price = 499.99M
                },
                new Product
                {
                    Name = "PlayStation VR2",
                    Description = "Next-generation virtual reality system for PS5 with 4K HDR visuals and eye tracking.",
                    Price = 549.99M
                },

                // Monitor
                new Product
                {
                    Name = "LG UltraGear 27\" Gaming Monitor",
                    Description = "QHD display with 165Hz refresh rate, 1ms response time, and G-Sync compatibility.",
                    Price = 399.99M
                },
                new Product
                {
                    Name = "Samsung Odyssey G7 32\" Curved Monitor",
                    Description = "Curved QHD gaming monitor with 240Hz refresh rate and 1ms response time.",
                    Price = 699.99M
                },

                // Keyboard
                new Product
                {
                    Name = "Logitech G Pro X Mechanical Keyboard",
                    Description = "Tournament-grade tenkeyless keyboard with hot-swappable switches and RGB lighting.",
                    Price = 149.99M
                },
                new Product
                {
                    Name = "Razer BlackWidow V3 Pro",
                    Description = "Wireless mechanical gaming keyboard with Razer green switches and multi-function digital dial.",
                    Price = 229.99M
                },

                // Mouse
                new Product
                {
                    Name = "Logitech G Pro X Superlight",
                    Description = "Ultra-lightweight wireless gaming mouse weighing less than 63 grams with HERO 25K sensor.",
                    Price = 159.99M
                },
                new Product
                {
                    Name = "Razer DeathAdder V3 Pro",
                    Description = "Ergonomic esports mouse with Focus Pro 30K optical sensor and 90-hour battery life.",
                    Price = 149.99M
                }
            );

            context.SaveChanges();
        }

        private static void SeedCategories(CatalogContext context)
        {
            context.Categories.AddRange(
                new Category { Name = "Console" },
                new Category { Name = "PC" },
                new Category { Name = "Laptop" },
                new Category { Name = "Controller" },
                new Category { Name = "Headset" },
                new Category { Name = "Accessory" },
                new Category { Name = "VR" },
                new Category { Name = "Monitor" },
                new Category { Name = "Keyboard" },
                new Category { Name = "Mouse" }
            );

            context.SaveChanges();
        }

        private static void SeedProductCategories(CatalogContext context)
        {
            var products = context.Products.ToList();
            var categories = context.Categories.ToList();

            Product FindProduct(string name) => products.First(p => p.Name == name);
            Category FindCategory(string name) => categories.First(c => c.Name == name);

            var productCategories = new List<ProductCategory>
            {
                // Console
                new() { Product = FindProduct("PlayStation 5"), Category = FindCategory("Console") },
                new() { Product = FindProduct("Xbox Series X"), Category = FindCategory("Console") },
                new() { Product = FindProduct("Nintendo Switch OLED"), Category = FindCategory("Console") },

                // PC
                new() { Product = FindProduct("Titan X Gaming Desktop"), Category = FindCategory("PC") },
                new() { Product = FindProduct("Challenger Gaming PC"), Category = FindCategory("PC") },

                // Laptop
                new() { Product = FindProduct("ROG Strix Gaming Laptop"), Category = FindCategory("Laptop") },
                new() { Product = FindProduct("Razer Blade 15"), Category = FindCategory("Laptop") },

                // Controller
                new() { Product = FindProduct("Xbox Elite Controller Series 2"), Category = FindCategory("Controller") },
                new() { Product = FindProduct("Xbox Elite Controller Series 2"), Category = FindCategory("Accessory") },
                new() { Product = FindProduct("DualSense Wireless Controller"), Category = FindCategory("Controller") },
                new() { Product = FindProduct("DualSense Wireless Controller"), Category = FindCategory("Accessory") },

                // Headset
                new() { Product = FindProduct("SteelSeries Arctis Nova Pro"), Category = FindCategory("Headset") },
                new() { Product = FindProduct("SteelSeries Arctis Nova Pro"), Category = FindCategory("Accessory") },
                new() { Product = FindProduct("HyperX Cloud Alpha"), Category = FindCategory("Headset") },
                new() { Product = FindProduct("HyperX Cloud Alpha"), Category = FindCategory("Accessory") },

                // Accessory
                new() { Product = FindProduct("Elgato Stream Deck"), Category = FindCategory("Accessory") },
                new() { Product = FindProduct("WD_BLACK 2TB External SSD"), Category = FindCategory("Accessory") },

                // VR
                new() { Product = FindProduct("Meta Quest 3"), Category = FindCategory("VR") },
                new() { Product = FindProduct("PlayStation VR2"), Category = FindCategory("VR") },
                new() { Product = FindProduct("PlayStation VR2"), Category = FindCategory("Accessory") },

                // Monitor
                new() { Product = FindProduct("LG UltraGear 27\" Gaming Monitor"), Category = FindCategory("Monitor") },
                new() { Product = FindProduct("Samsung Odyssey G7 32\" Curved Monitor"), Category = FindCategory("Monitor") },

                // Keyboard
                new() { Product = FindProduct("Logitech G Pro X Mechanical Keyboard"), Category = FindCategory("Keyboard") },
                new() { Product = FindProduct("Logitech G Pro X Mechanical Keyboard"), Category = FindCategory("Accessory") },
                new() { Product = FindProduct("Razer BlackWidow V3 Pro"), Category = FindCategory("Keyboard") },
                new() { Product = FindProduct("Razer BlackWidow V3 Pro"), Category = FindCategory("Accessory") },

                // Mouse
                new() { Product = FindProduct("Logitech G Pro X Superlight"), Category = FindCategory("Mouse") },
                new() { Product = FindProduct("Logitech G Pro X Superlight"), Category = FindCategory("Accessory") },
                new() { Product = FindProduct("Razer DeathAdder V3 Pro"), Category = FindCategory("Mouse") },
                new() { Product = FindProduct("Razer DeathAdder V3 Pro"), Category = FindCategory("Accessory") }
            };

            context.ProductCategories.AddRange(productCategories);
            context.SaveChanges();
        }
    }
}