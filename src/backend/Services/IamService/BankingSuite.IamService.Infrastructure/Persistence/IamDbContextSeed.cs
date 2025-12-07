using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.Infrastructure.Persistence;

public static class IamDbContextSeed
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IamDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Apply pending migrations (safe to call multiple times)
        await context.Database.MigrateAsync();

        string[] roles =
        [
            "Employee",
            "IamAdmin",
            "SuperAdmin"
        ];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        const string adminEmail = "admin@alvorbank.test";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Alvor",
                LastName = "Bank IAM Admin",
                EmailConfirmed = true,
                IsActive = true,
                TwoFactorEnabled = false
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create default admin user: {errors}");
            }
        }

        var desiredRoles = new[] { "Employee", "IamAdmin", "SuperAdmin" };
        var currentRoles = await userManager.GetRolesAsync(admin);
        var missingRoles = desiredRoles.Except(currentRoles).ToArray();

        if (missingRoles.Length > 0)
        {
            var addRolesResult = await userManager.AddToRolesAsync(admin, missingRoles);

            if (!addRolesResult.Succeeded)
            {
                var errors = string.Join("; ", addRolesResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign roles to admin user: {errors}");
            }
        }
    }
}
