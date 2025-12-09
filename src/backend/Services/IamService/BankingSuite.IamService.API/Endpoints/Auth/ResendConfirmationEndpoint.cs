using BankingSuite.IamService.Application.Auth.Commands.ResendConfirmation;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class ResendConfirmationRequest
{
    public string Email { get; init; } = string.Empty;
}

public sealed class ResendConfirmationResponse
{
    public bool Sent { get; init; }
}

public sealed class ResendConfirmationEndpoint(IMediator mediator, IConfiguration configuration)
    : Endpoint<ResendConfirmationRequest, ResendConfirmationResponse>
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;

    public override void Configure()
    {
        Post("/api/iam/auth/resend-confirmation");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Resend email confirmation link to an employee.";
        });
    }

    public override async Task HandleAsync(ResendConfirmationRequest req, CancellationToken ct)
    {
        // This could come from configuration. Example:
        // "Iam:EmailConfirmationBaseUrl": "https://alvorbank.dev/iam/email-confirmation"
        var baseUrl =
            _configuration["Iam:EmailConfirmationBaseUrl"]
            ?? "https://localhost:4200/email-confirmation";

        var cmd = new ResendConfirmationEmailCommand(
            Email: req.Email,
            ConfirmationBaseUrl: baseUrl
        );

        await _mediator.Send(cmd, ct);

        // We intentionally do NOT reveal whether the email exists.
        await Send.OkAsync(new ResendConfirmationResponse { Sent = true }, ct);
    }
}
