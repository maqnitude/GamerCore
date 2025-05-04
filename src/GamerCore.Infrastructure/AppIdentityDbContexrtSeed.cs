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
        private const string AdminEmail = "admin@example.com";
        private const string AdminPassword = "Secret123$";
        private const string AdminUserName = AdminEmail;

        public static async void EnsurePopulated(IApplicationBuilder app)
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

            var user = await userManager.FindByNameAsync(AdminUserName);
            if (user == null)
            {
                user = new AppUser
                {
                    FirstName = "John",
                    LastName = "Admin",
                    UserName = AdminUserName,
                    Email = AdminEmail,
                    PhoneNumber = "555-1234"
                };

                await userManager.CreateAsync(user, AdminPassword);
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }
    }
}