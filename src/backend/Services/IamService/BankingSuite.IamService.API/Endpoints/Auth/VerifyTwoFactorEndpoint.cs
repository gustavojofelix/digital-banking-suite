using BankingSuite.IamService.Application.Auth.Commands.Login;
using BankingSuite.IamService.Application.Auth.Commands.VerifyTwoFactor;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class VerifyTwoFactorRequest
{
    public Guid UserId { get; init; }
    public string Code { get; init; } = string.Empty;
}

public sealed class VerifyTwoFactorEndpoint(IMediator mediator)
    : Endpoint<VerifyTwoFactorRequest, LoginResult>
{
    public override void Configure()
    {
        Post("/api/iam/auth/2fa/verify");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Verify a 2FA code sent via email and issue a JWT for the employee.";
        });
    }

    public override async Task HandleAsync(VerifyTwoFactorRequest req, CancellationToken ct)
    {
        var cmd = new VerifyTwoFactorCommand(UserId: req.UserId, Code: req.Code);

        var result = await mediator.Send(cmd, ct);

        await Send.OkAsync(result, ct);
    }
}
