using BankingSuite.IamService.Infrastructure;
using BankingSuite.IamService.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger; // 👈 important for SwaggerDocument / UseSwaggerGen

var builder = WebApplication.CreateBuilder(args);

// FastEndpoints + Swagger (with JWT auth enabled by default)
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        // Swagger/OpenAPI metadata
        o.DocumentSettings = s =>
        {
            s.Title = "Alvor Bank - IAM Service";
            s.Version = "v1";
        };

        // JWT bearer auth is enabled by default:
        // o.EnableJWTBearerAuth = true; // (default = true)
    });

// IAM infrastructure (Identity + EF Core + JWT + MediatR)
builder.Services.AddIamInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply migrations and seed IAM data (roles + default admin)
await IamDbContextSeed.SeedAsync(app.Services);

// existing middleware/endpoint configuration
app.UseAuthentication();
app.UseAuthorization();

// FastEndpoints must come before SwaggerGen
app.UseFastEndpoints();

// Enable Swagger UI (dev only)
if (app.Environment.IsDevelopment())
{
    // FastEndpoints helper: adds OpenAPI + Swagger UI with sane defaults
    app.UseSwaggerGen();
}

app.Run();

// For WebApplicationFactory in test
public partial class Program;
