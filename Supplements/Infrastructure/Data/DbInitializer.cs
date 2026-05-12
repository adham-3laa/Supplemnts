using Microsoft.AspNetCore.Identity;
using Supplements.Core.Entities;
using Supplements.Infrastructure.Data;

namespace Supplements.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        string[] roleNames = { "Admin", "Seller", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        // Seed Admin User
        var adminEmail = "a@a.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, "123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
