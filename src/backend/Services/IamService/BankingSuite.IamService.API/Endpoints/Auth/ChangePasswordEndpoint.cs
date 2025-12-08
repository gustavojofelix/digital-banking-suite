using BankingSuite.IamService.Application.Auth.Commands.ChangePassword;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ChangePasswordEndpoint(IMediator mediator) : Endpoint<ChangePasswordRequest>
{
    public override void Configure()
    {
        Post("/api/iam/auth/change-password");
        Roles("Employee", "IamAdmin", "SuperAdmin"); // adjust to match your roles
        Summary(s =>
        {
            s.Summary = "Change password for the currently logged-in employee.";
        });
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var cmd = new ChangePasswordCommand(
            CurrentPassword: req.CurrentPassword,
            NewPassword: req.NewPassword
        );

        await mediator.Send(cmd, ct);

        await Send.NoContentAsync(ct);
    }
}
