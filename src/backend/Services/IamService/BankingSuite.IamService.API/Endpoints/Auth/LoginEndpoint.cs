//using BankingSuite.IamService.Application.Auth;
//using BankingSuite.IamService.Application.Auth.Dto;
//using FastEndpoints;
//using Microsoft.AspNetCore.Http;
//using MediatR;

//namespace BankingSuite.IamService.API.Endpoints.Auth;

//public class LoginRequest
//{
//    public string Email { get; set; } = default!;
//    public string Password { get; set; } = default!;
//}

//public class LoginResponse
//{
//    public string AccessToken { get; set; } = default!;
//    public DateTime ExpiresAt { get; set; }
//}

//public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
//{
//    private readonly IMediator _mediator;

//    public LoginEndpoint(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    public override void Configure()
//    {
//        Post("/auth/login");
//        AllowAnonymous();
//        Summary(s =>
//        {
//            s.Summary = "Login and receive a JWT access token";
//        });
//    }

//    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
//    {
//        var result = await _mediator.Send(new LoginCommand(req.Email, req.Password), ct);

//        if (result is null)
//        {
//            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await HttpContext.Response.CompleteAsync();
//            return;
//        }

//        HttpContext.Response.StatusCode = StatusCodes.Status200OK;
//        await HttpContext.Response.WriteAsJsonAsync(new LoginResponse
//        {
//            AccessToken = result.AccessToken,
//            ExpiresAt = result.ExpiresAt
//        }, cancellationToken: ct);
//    }
//}
