using FastEndpoints;
using Microsoft.AspNetCore.Builder;

namespace BankingSuite.BuildingBlocks.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseBankingFastEndpoints(this WebApplication app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
        });

        app.UseOpenApi();
        app.UseSwaggerUi();

        return app;
    }
}