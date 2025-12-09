using BankingSuite.IamService.Application.Auth.Commands.ResetPassword;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class ResetPasswordRequest
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public sealed class ResetPasswordEndpoint(IMediator mediator) : Endpoint<ResetPasswordRequest>
{
    public override void Configure()
    {
        Post("/api/iam/auth/reset-password");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Complete the password reset flow using the email token.";
        });
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var cmd = new ResetPasswordCommand(
            Email: req.Email,
            Token: req.Token,
            NewPassword: req.NewPassword
        );

        await mediator.Send(cmd, ct);

        await Send.NoContentAsync(ct);
    }
}
