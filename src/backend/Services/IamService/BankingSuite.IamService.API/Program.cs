using BankingSuite.IamService.API.Common;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Infrastructure;
using BankingSuite.IamService.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger; // 👈 important for SwaggerDocument / UseSwaggerGen

var builder = WebApplication.CreateBuilder(args);

// CORS: allow Angular dev origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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

// 👇 add these two lines
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var app = builder.Build();

// CORS must come before auth + FastEndpoints
app.UseCors("FrontendDev");

// existing middleware/endpoint configuration
app.UseAuthentication();
app.UseAuthorization();

// FastEndpoints must come before SwaggerGen
app.UseFastEndpoints();

// Enable Swagger UI (dev only)
if (app.Environment.IsDevelopment())
{
    //// Apply migrations and seed IAM data (roles + default admin)
    //await IamDbContextSeed.SeedAsync(app.Services);

    // FastEndpoints helper: adds OpenAPI + Swagger UI with sane defaults
    app.UseSwaggerGen();
}

// Apply migrations and seed IAM data (roles + default admin)
// ⚠️ Skip this when running integration tests so we don't need a real Postgres instance
if (!app.Environment.IsEnvironment("IntegrationTests"))
{
    await IamDbContextSeed.SeedAsync(app.Services);
}

app.Run();

// For WebApplicationFactory in test
public partial class Program;
