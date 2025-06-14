using System.Net;
using System.Text;
using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GamerCore.Infrastructure.Data
{
    public static class ApplicationDbContextSeed
    {
        public const string AdminEmail = "admin@example.com";

        private const string AdminPassword = "Secret123$";
        private const string AdminUserName = AdminEmail;

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            // Important: Do not change the order
            await SeedUserRolesAsync(roleManager);
            await SeedAdminAsync(userManager);

            if (environment.IsDevelopment())
            {
                await SeedCustomersAsync(userManager);

                if (!dbContext.Categories.Any())
                {
                    SeedCategories(dbContext);
                }

                if (!dbContext.Products.Any())
                {
                    SeedProducts(dbContext);
                }

                if (!dbContext.ProductDetails.Any())
                {
                    SeedProductDetails(dbContext);
                }

                if (!dbContext.ProductImages.Any())
                {
                    SeedProductImages(dbContext);
                }

                if (!dbContext.ProductReviews.Any())
                {
                    SeedProductReviews(dbContext, userManager);
                }
            }
        }

        private static async Task SeedUserRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(RoleConstants.Admin);
            if (adminRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Admin));
            }

            var customerRole = await roleManager.FindByNameAsync(RoleConstants.Customer);
            if (customerRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Customer));
            }
        }

        private static async Task SeedAdminAsync(UserManager<User> userManager)
        {
            var admin = await userManager.FindByNameAsync(AdminUserName);
            if (admin == null)
            {
                admin = new User
                {
                    FirstName = "John",
                    LastName = "Admin",
                    UserName = AdminUserName,
                    Email = AdminEmail,
                    PhoneNumber = "555-1234"
                };

                await userManager.CreateAsync(admin, AdminPassword);
                await userManager.AddToRoleAsync(admin, RoleConstants.Admin);
            }
        }

        private static async Task SeedCustomersAsync(UserManager<User> userManager)
        {
            var customers = new List<User>
            {
                new() { FirstName = "John", LastName = "Doe" },
                new() { FirstName = "Jane", LastName = "Doe" },
                new() { FirstName = "John", LastName = "Wick" },
                new() { FirstName = "John", LastName = "Smith" },
                new() { FirstName = "Donald", LastName = "Trump" },
                new() { FirstName = "Alice", LastName = "Johnson" },
                new() { FirstName = "Bob", LastName = "Anderson"},
                new() { FirstName = "Charlie", LastName = "Brown" },
                new() { FirstName = "Diana", LastName = "Prince" },
                new() { FirstName = "Evan", LastName = "Taylor" }
            };

            foreach (var customer in customers)
            {
                string customerEmail = $"{customer.FirstName}.{customer.LastName}@example.com".ToLower();
                string customerPassword = $"{customer.FirstName}123$";
                var existingCustomer = await userManager.FindByNameAsync(customerEmail);
                if (existingCustomer == null)
                {
                    customer.UserName = customerEmail;
                    customer.Email = customerEmail;

                    await userManager.CreateAsync(customer, customerPassword);
                    await userManager.AddToRoleAsync(customer, RoleConstants.Customer);
                }
            }
        }

        private static void SeedCategories(ApplicationDbContext dbContext)
        {
            var categories = new List<Category>
            {
                new() { Name = "Consoles", Description = "Video game consoles are specialized electronic devices designed to connect to a display and run video games via dedicated hardware and controllers, offering immersive entertainment experiences in a living room setting." },
                new() { Name = "PCs", Description = "Gaming PCs are high-performance desktop computers equipped with powerful multicore CPUs, dedicated graphics cards, and ample memory to handle demanding games and applications, with the flexibility to upgrade components over time." },
                new() { Name = "Laptops", Description = "Portable computers that integrate the display, keyboard, battery, and computing hardware into a single, compact chassis, delivering mobility and versatility for work, study, and entertainment." },
                new() { Name = "Controllers", Description = "Input devices featuring buttons, triggers, joysticks, and motion sensors, designed to provide precise in-game control and interaction across gaming consoles and PCs." },
                new() { Name = "Headsets", Description = "Integrated headphone-and-microphone peripherals optimized for gaming and communication, offering surround sound, noise cancellation, and clear voice chat to enhance immersion and teamwork." },
                new() { Name = "Monitors", Description = "Output devices that display visual content from computers and consoles; modern monitors typically use LCD, LED, or OLED panels with high resolutions, fast refresh rates, and accurate color reproduction for gaming and multimedia." },
                new() { Name = "Keyboards", Description = "Input devices composed of sets of keys for typing text and commands; gaming keyboards often feature mechanical switches, customizable RGB lighting, programmable macros, and ergonomic designs for performance and comfort." },
                new() { Name = "Mice", Description = "Handheld pointing devices that translate surface movement into cursor motion; gaming mice include features like adjustable DPI, programmable buttons, ergonomic shapes, and high-precision optical or laser sensors for accurate control." }
            };

            dbContext.Categories.AddRange(categories);
            dbContext.SaveChanges();
        }

        private static void SeedProducts(ApplicationDbContext dbContext)
        {
            var categories = dbContext.Categories.ToList();
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
                        Price = price,
                        IsFeatured = i == 1 // Feature the first product of each category
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

            dbContext.Products.AddRange(products);
            dbContext.ProductCategories.AddRange(productCategories);
            dbContext.SaveChanges();
        }

        private static void SeedProductDetails(ApplicationDbContext dbContext)
        {
            var products = dbContext.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .ToList();
            var productDetails = new List<ProductDetail>();

            // Define category-specific description templates
            var descriptionTemplates = new Dictionary<string, string>
            {
                { "Consoles", "<h3>Next-Gen Gaming Console</h3><p>Experience gaming like never before with the {0}. " +
                    "This powerful console delivers stunning graphics, lightning-fast load times, and an " +
                    "extensive library of games.</p><ul><li>4K gaming at up to 120fps</li>" +
                    "<li>High-speed SSD storage</li><li>Ray tracing technology</li>" +
                    "<li>Backward compatibility with classic titles</li></ul>" },

                { "PCs", "<h3>High-Performance Gaming PC</h3><p>Dominate the competition with the {0}. " +
                    "Built for serious gamers, this custom rig delivers exceptional performance for even " +
                    "the most demanding titles.</p><ul><li>Latest-gen processor</li>" +
                    "<li>Premium graphics card</li><li>Ultra-fast RAM</li>" +
                    "<li>Advanced cooling system</li></ul>" },

                { "Laptops", "<h3>Portable Gaming Powerhouse</h3><p>Take your gaming anywhere with the {0}. " +
                    "This laptop combines mobility with desktop-class performance for gaming on the go.</p>" +
                    "<ul><li>High-refresh display</li><li>Long battery life</li>" +
                    "<li>Efficient thermal design</li><li>Customizable RGB lighting</li></ul>" },

                { "Controllers", "<h3>Precision Gaming Controller</h3><p>Gain the competitive edge with the {0}. " +
                    "Designed for comfort and precision during extended gaming sessions.</p>" +
                    "<ul><li>Ergonomic design</li><li>Customizable buttons</li>" +
                    "<li>Low input latency</li><li>Long battery life</li></ul>" },

                { "Headsets", "<h3>Immersive Gaming Audio</h3><p>Hear every detail with the {0}. " +
                    "This premium headset delivers crystal-clear sound and communication for total gaming immersion.</p>" +
                    "<ul><li>Surround sound technology</li><li>Noise-cancelling microphone</li>" +
                    "<li>Memory foam ear cushions</li><li>Cross-platform compatibility</li></ul>" },

                { "Monitors", "<h3>Ultra-Responsive Gaming Display</h3><p>See the difference with the {0}. " +
                    "This monitor is designed specifically for competitive gaming with minimal latency and maximum clarity.</p>" +
                    "<ul><li>High refresh rate</li><li>1ms response time</li>" +
                    "<li>Adaptive sync technology</li><li>Anti-glare coating</li></ul>" },

                { "Keyboards", "<h3>Tactical Gaming Keyboard</h3><p>Every keystroke matters with the {0}. " +
                    "Engineered for speed, durability, and precision in competitive gaming scenarios.</p>" +
                    "<ul><li>Mechanical switches</li><li>N-key rollover</li>" +
                    "<li>Programmable macros</li><li>Dynamic RGB lighting</li></ul>" },

                { "Mice", "<h3>Precision Gaming Mouse</h3><p>Gain pixel-perfect accuracy with the {0}. " +
                    "Designed for competitive gamers who demand the ultimate in precision and speed.</p>" +
                    "<ul><li>High-precision optical sensor</li><li>Adjustable DPI settings</li>" +
                    "<li>Lightweight design</li><li>Customizable buttons</li></ul>" }
            };

            // Define category-specific warranty templates
            var warrantyTemplates = new Dictionary<string, string>
            {
                { "Consoles", "<h4>Limited Hardware Warranty</h4><p>Your {0} includes a 2-year limited hardware " +
                    "warranty covering defects in materials and workmanship under normal use.</p>" +
                    "<p>This warranty does not cover damage from accidents, misuse, or unauthorized modifications.</p>" },

                { "PCs", "<h4>Premium System Warranty</h4><p>Your {0} includes a 3-year comprehensive warranty " +
                    "with parts and labor coverage.</p><p>First year includes on-site service for hardware issues. " +
                    "24/7 technical support available throughout warranty period.</p>" },

                { "Laptops", "<h4>Mobile Computing Warranty</h4><p>Your {0} includes a 2-year limited warranty " +
                    "covering manufacturing defects.</p><p>First year includes accidental damage protection. " +
                    "Battery covered for 1 year under separate warranty terms.</p>" },

                { "Controllers", "<h4>Accessory Warranty</h4><p>Your {0} includes a 1-year limited warranty " +
                    "against manufacturing defects.</p><p>Normal wear and tear not covered. " +
                    "Extended protection plans available at checkout.</p>" },

                { "Headsets", "<h4>Audio Accessory Warranty</h4><p>Your {0} includes a 1-year limited warranty " +
                    "covering electrical components and manufacturing defects.</p><p>Cables and ear cushions " +
                    "covered for 6 months under normal use conditions.</p>" },

                { "Monitors", "<h4>Display Warranty</h4><p>Your {0} includes a 3-year limited warranty covering " +
                    "panel defects, including dead pixels exceeding manufacturer guidelines.</p>" +
                    "<p>Advanced replacement available in first year of ownership.</p>" },

                { "Keyboards", "<h4>Input Device Warranty</h4><p>Your {0} includes a 2-year limited warranty " +
                    "covering switch failures and electronic components.</p><p>Keycaps covered for " +
                    "6 months against premature wear or legends fading.</p>" },

                { "Mice", "<h4>Precision Device Warranty</h4><p>Your {0} includes a 2-year limited warranty " +
                    "covering sensor failures and button malfunctions.</p><p>Mouse feet and cable covered " +
                    "for 1 year against abnormal wear.</p>" }
            };

            foreach (var product in products)
            {
                // Get the category of the product
                var category = product.ProductCategories.FirstOrDefault()?.Category;
                if (category == null)
                {
                    continue;
                }

                var descriptionTemplate = descriptionTemplates[category.Name];
                var warrantyTemplate = warrantyTemplates[category.Name];

                // Create product-specific description and warranty
                var description = string.Format(descriptionTemplate, product.Name);
                var warranty = string.Format(warrantyTemplate, product.Name);

                // Add some randomized additional information based on price
                var additionalFeatures = GenerateAdditionalDetail(product.Price, category.Name);
                description += additionalFeatures;

                productDetails.Add(new ProductDetail
                {
                    ProductId = product.Id,
                    DescriptionHtml = description,
                    WarrantyHtml = warranty
                });
            }

            dbContext.ProductDetails.AddRange(productDetails);
            dbContext.SaveChanges();
        }

        private static void SeedProductImages(ApplicationDbContext dbContext)
        {
            var products = dbContext.Products.ToList();

            var productImages = new List<ProductImage>();
            const int imageCount = 5;
            const string baseUrl = "https://placehold.co/600x400";

            foreach (var product in products)
            {
                for (int i = 1; i <= imageCount; i++)
                {
                    bool isPrimary = i == 1; // First image is primary

                    // Construct the text, adding "(primary)" if needed
                    string baseText = $"{product.Name} #{i}";
                    string imageText = isPrimary ? $"{baseText}\n(primary)" : baseText;

                    // URL encode the text to handle spaces, '#', and other special characters
                    string encodedText = WebUtility.UrlEncode(imageText);

                    // Construct the full URL
                    string imageUrl = $"{baseUrl}?text={encodedText}";

                    productImages.Add(new ProductImage
                    {
                        Url = imageUrl,
                        IsPrimary = isPrimary,
                        ProductId = product.Id // Link the image to the product
                    });
                }
            }

            dbContext.ProductImages.AddRange(productImages);
            dbContext.SaveChanges();
        }

        private static void SeedProductReviews(ApplicationDbContext context, UserManager<User> userManager)
        {
            var products = context.Products.ToList();
            var userIds = userManager.Users
                // Quick and dirty way to filter out admin
                .Where(u => u.Email != AdminEmail)
                .Select(u => u.Id)
                .ToList();

            var reviews = new List<ProductReview>();
            var random = new Random();

            int reviewId = 1;

            foreach (var product in products)
            {
                int reviewCount = random.Next(1, Math.Min(11, userIds.Count + 1));

                var shuffledUserIds = userIds
                    .OrderBy(_ => random.Next())
                    .Take(reviewCount)
                    .ToList();

                foreach (var userId in shuffledUserIds)
                {
                    int rating = random.Next(1, 6);

                    bool hasReview = random.NextDouble() < 0.5;
                    string? reviewTitle = hasReview ? $"Title of the review {reviewId++}" : null;
                    string? reviewText = hasReview
                        ? $"This is the review text of this customer on this product {reviewId}"
                        : null;

                    reviews.Add(new ProductReview
                    {
                        ProductId = product.Id,
                        Rating = rating,
                        ReviewTitle = reviewTitle,
                        ReviewText = reviewText,
                        UserId = userId
                    });
                }
            }

            context.ProductReviews.AddRange(reviews);
            context.SaveChanges();
        }

        private static string GenerateAdditionalDetail(decimal price, string categoryName)
        {
            var features = new StringBuilder();
            features.Append("<h3>Additional Features</h3>");

            // Add premium features based on price ranges
            if (categoryName == "Consoles" || categoryName == "PCs" || categoryName == "Laptops")
            {
                if (price > 2000)
                {
                    features.Append("<p><strong>Premium Tier Features:</strong></p><ul>");
                    features.Append("<li>Extended 5-year warranty available</li>");
                    features.Append("<li>Priority technical support</li>");
                    features.Append("<li>VIP gaming subscription included (1 year)</li>");
                    features.Append("</ul>");
                }
                else if (price > 1000)
                {
                    features.Append("<p><strong>Enhanced Features:</strong></p><ul>");
                    features.Append("<li>Extended 3-year warranty available</li>");
                    features.Append("<li>Gaming subscription included (3 months)</li>");
                    features.Append("</ul>");
                }
            }
            else if (categoryName == "Monitors")
            {
                if (price > 500)
                {
                    features.Append("<p><strong>Professional Display Features:</strong></p><ul>");
                    features.Append("<li>HDR support</li>");
                    features.Append("<li>99% color accuracy</li>");
                    features.Append("<li>Built-in USB hub</li>");
                    features.Append("</ul>");
                }
            }
            else if (categoryName == "Controllers" || categoryName == "Headsets" ||
                     categoryName == "Keyboards" || categoryName == "Mice")
            {
                if (price > 100)
                {
                    features.Append("<p><strong>Pro Series Features:</strong></p><ul>");
                    features.Append("<li>Aircraft-grade aluminum construction</li>");
                    features.Append("<li>Customizable profiles</li>");
                    features.Append("<li>Carrying case included</li>");
                    features.Append("</ul>");
                }
            }

            return features.ToString();
        }
    }
}