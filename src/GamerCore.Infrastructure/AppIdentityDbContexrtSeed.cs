using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamerCore.Infrastructure
{
    public static class AppIdentityDbContextSeed
    {
        public const string AdminEmail = "admin@example.com";

        private const string AdminPassword = "Secret123$";
        private const string AdminUserName = AdminEmail;

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();
            var roleManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            // Seed roles
            var adminRole = await roleManager.FindByNameAsync(RoleNames.Administrator);
            if (adminRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RoleNames.Administrator));
            }

            var userRole = await roleManager.FindByNameAsync(RoleNames.User);
            if (userRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RoleNames.User));
            }

            // Seed admin
            var admin = await userManager.FindByNameAsync(AdminUserName);
            if (admin == null)
            {
                admin = new AppUser
                {
                    FirstName = "John",
                    LastName = "Admin",
                    UserName = AdminUserName,
                    Email = AdminEmail,
                    PhoneNumber = "555-1234"
                };

                await userManager.CreateAsync(admin, AdminPassword);
                await userManager.AddToRoleAsync(admin, RoleNames.Administrator);
            }

            // Seed customers
            var customers = new List<AppUser>
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
                    await userManager.AddToRoleAsync(customer, RoleNames.User);
                }
            }
        }
    }
}