using GamerCore.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamerCore.Infrastructure
{
    public static class AppIdentityDbContextSeed
    {
        private const string _adminEmail = "admin@example.com";
        private const string _adminUserName = _adminEmail;
        private const string _adminPassword = "Secret123$";

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

            var user = await userManager.FindByNameAsync(_adminUserName);

            if (user == null)
            {
                user = new AppUser(_adminUserName)
                {
                    FirstName = "John",
                    LastName = "Admin",
                    Email = _adminEmail,
                    PhoneNumber = "555-1234"
                };
                await userManager.CreateAsync(user, _adminPassword);
            }
        }
    }
}