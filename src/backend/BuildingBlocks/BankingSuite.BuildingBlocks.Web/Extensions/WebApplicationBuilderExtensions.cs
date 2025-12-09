using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.BuildingBlocks.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddBankingFastEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        services.SwaggerDocument();
        return services;
    }
}
