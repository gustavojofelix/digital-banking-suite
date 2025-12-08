using BankingSuite.IamService.Application.Auth.Commands.ConfirmEmail;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class ConfirmEmailRequest
{
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
}

public sealed class ConfirmEmailEndpoint(IMediator mediator) : Endpoint<ConfirmEmailRequest>
{
    public override void Configure()
    {
        Get("/api/iam/auth/confirm-email");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Confirm employee email address using the link sent via email.";
        });
    }

    public override async Task HandleAsync(ConfirmEmailRequest req, CancellationToken ct)
    {
        var cmd = new ConfirmEmailCommand(req.UserId, req.Token);

        await mediator.Send(cmd, ct);

        // For now, just return 204. Later we can redirect to a frontend "Email confirmed" page.
        await Send.NoContentAsync(ct);
    }
}
