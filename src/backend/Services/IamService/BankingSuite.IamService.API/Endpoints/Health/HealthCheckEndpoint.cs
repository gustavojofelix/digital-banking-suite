using FastEndpoints;

namespace BankingSuite.IamService.API.Endpoints.Health;

public class HealthCheckEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/health");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Simple health check for the IAM service.";
            s.Description = "Returns a basic payload that tells if the IAM service is running.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(new
        {
            Service = "IamService",
            Status = "Healthy",
            TimestampUtc = DateTime.UtcNow
        }, cancellation: ct);
    }
}
