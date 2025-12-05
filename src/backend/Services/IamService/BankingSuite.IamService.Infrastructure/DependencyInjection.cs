using BankingSuite.BuildingBlocks.Application.Abstractions;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.Infrastructure.Auth;
using BankingSuite.IamService.Infrastructure.Identity;
using BankingSuite.IamService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankingSuite.IamService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IamDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IamDatabase"));
        });

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<IamDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddScoped<IIdentityService, IdentityService>();

        // Simple UnitOfWork for IAM (for future additional entities)
        services.AddScoped<IUnitOfWork, IamUnitOfWork>();

        return services;
    }

    public static async Task SeedIdentityAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var roleName in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });

                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        const string adminEmail = "admin@alvorbank.co.mz";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = adminEmail,
                UserName = adminEmail,
                DisplayName = "System Administrator",
                UserType = UserType.Employee,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            var roleResult = await userManager.AddToRoleAsync(adminUser, RoleNames.SystemAdmin);
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to assign SystemAdmin role: {Errors}",
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            logger.LogInformation("Seeded SystemAdmin user with email {Email}", adminEmail);
        }
    }
}

public class IamUnitOfWork : IUnitOfWork
{
    private readonly IamDbContext _dbContext;

    public IamUnitOfWork(IamDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
