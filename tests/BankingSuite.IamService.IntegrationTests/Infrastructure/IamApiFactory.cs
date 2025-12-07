using BankingSuite.IamService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.IntegrationTests.Infrastructure;

public sealed class IamApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices(services =>
        {
            // OPTIONAL: if you want, you can swap to in-memory DB here later.
            // For now you can leave the real DB registration as-is,
            // since we are skipping seeding in this environment.
            //
            // Example (uncomment to use in-memory instead of Postgres):
            //
            // var dbDescriptor = services.SingleOrDefault(
            //     d => d.ServiceType == typeof(DbContextOptions<IamDbContext>));
            //
            // if (dbDescriptor is not null)
            //     services.Remove(dbDescriptor);
            //
            // services.AddDbContext<IamDbContext>(options =>
            // {
            //     options.UseInMemoryDatabase("iam_integration_tests_db");
            // });
        });
    }
}
