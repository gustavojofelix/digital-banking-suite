using BankingSuite.IamService.Application.Auth.Commands.ForgotPassword;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public sealed class ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
}

public sealed class ForgotPasswordResponse
{
    public bool Sent { get; init; }
}

public sealed class ForgotPasswordEndpoint(IMediator mediator, IConfiguration configuration)
    : Endpoint<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;

    public override void Configure()
    {
        Post("/api/iam/auth/forgot-password");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Start the password reset flow for an employee.";
        });
    }

    public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
    {
        var baseUrl =
            _configuration["Iam:PasswordResetBaseUrl"] ?? "https://localhost:4200/password-reset";

        var cmd = new ForgotPasswordCommand(Email: req.Email, ResetBaseUrl: baseUrl);

        await _mediator.Send(cmd, ct);

        await Send.OkAsync(new ForgotPasswordResponse { Sent = true }, ct);
    }
}
