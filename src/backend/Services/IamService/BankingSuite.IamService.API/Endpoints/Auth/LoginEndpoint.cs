using BankingSuite.IamService.Application.Auth.Commands.Login;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Login and receive a JWT access token.";
            s.Description = "Validates credentials and issues a JWT used by other microservices.";
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new LoginCommand(req.Email, req.Password),
            ct);

        if (result.IsFailure)
        {
            AddError(result.Error ?? "Login failed.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var value = result.Value!;

        await Send.OkAsync(new LoginResponse
        {
            AccessToken = value.AccessToken,
            ExpiresAtUtc = value.ExpiresAtUtc
        }, cancellation: ct);
    }
}
