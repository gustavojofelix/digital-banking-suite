using System.Net;
using BankingSuite.IamService.Application.Auth.Commands.Login;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public class LoginEndpoint : Endpoint<LoginRequest, LoginResult>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/iam/auth/login", "/auth/login");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Login and receive a JWT access token.";
            s.Description = "Validates credentials and issues a JWT used by other microservices.";
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(
                new LoginCommand(req.Email, req.Password),
                ct);

            await Send.OkAsync(result, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await HttpContext.Response.WriteAsJsonAsync(
                new { Errors = new[] { ex.Message } },
                cancellationToken: ct);
        }
    }
}
