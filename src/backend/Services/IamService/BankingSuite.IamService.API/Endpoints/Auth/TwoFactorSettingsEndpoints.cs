using BankingSuite.IamService.Application.Auth.Commands.TwoFactor;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class TwoFactorUpdateRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
}

public sealed class EnableTwoFactorEndpoint(IMediator mediator) : Endpoint<TwoFactorUpdateRequest>
{
    public override void Configure()
    {
        Post("/api/iam/auth/2fa/enable");
        Roles("Employee", "IamAdmin", "SuperAdmin"); // adjust role names as needed
        Summary(s =>
        {
            s.Summary = "Enable 2FA for the current employee using their password.";
        });
    }

    public override async Task HandleAsync(TwoFactorUpdateRequest req, CancellationToken ct)
    {
        await mediator.Send(new EnableTwoFactorCommand(req.CurrentPassword), ct);
        await Send.NoContentAsync(ct);
    }
}

public sealed class DisableTwoFactorEndpoint(IMediator mediator) : Endpoint<TwoFactorUpdateRequest>
{
    public override void Configure()
    {
        Post("/api/iam/auth/2fa/disable");
        Roles("Employee", "IamAdmin", "SuperAdmin");
        Summary(s =>
        {
            s.Summary = "Disable 2FA for the current employee using their password.";
        });
    }

    public override async Task HandleAsync(TwoFactorUpdateRequest req, CancellationToken ct)
    {
        await mediator.Send(new DisableTwoFactorCommand(req.CurrentPassword), ct);
        await Send.NoContentAsync(ct);
    }
}
