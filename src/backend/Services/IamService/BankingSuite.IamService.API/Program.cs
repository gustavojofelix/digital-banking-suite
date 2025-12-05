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

// Minimal health endpoint (no auth yet)
//app.MapGet("/health", () => Results.Ok(new
//{
//    Service = "IamService",
//    Status = "Healthy",
//    TimestampUtc = DateTime.UtcNow
//}));

app.Run();

public partial class Program;
