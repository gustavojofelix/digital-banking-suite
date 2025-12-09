using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Helpers;

internal static class IdentityTestHelper
{
    public static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDataProtection();

        services.AddDbContext<IamDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<ApplicationRole>() 
            .AddEntityFrameworkStores<IamDbContext>()
            .AddDefaultTokenProviders();

        return services.BuildServiceProvider();
    }
}
