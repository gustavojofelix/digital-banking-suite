using BankingSuite.IamService.Infrastructure;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// 🔧 Add this line so Swagger/OpenAPI has API Explorer
builder.Services.AddEndpointsApiExplorer();
// Add Swagger for FastEndpoints
builder.Services.AddSwaggerDocument(config =>
{
    config.Title = "Alvor Bank - IAM Service";
    config.Version = "v1";
});

// IAM infrastructure (Identity + EF Core + JWT + MediatR)
builder.Services.AddIamInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(settings =>
    {
        settings.Path = "/swagger";
        settings.DocumentPath = "/swagger/v1/swagger.json";
    });
}

app.Run();

// For WebApplicationFactory in test
public partial class Program;
