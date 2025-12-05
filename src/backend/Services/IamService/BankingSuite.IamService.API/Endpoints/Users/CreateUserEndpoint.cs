//using BankingSuite.IamService.Application.Users;
//using BankingSuite.IamService.Application.Users.Dto;
//using BankingSuite.IamService.Domain.Users;
//using FastEndpoints;
//using Microsoft.AspNetCore.Http;
//using MediatR;

//namespace BankingSuite.IamService.API.Endpoints.Users;

//public class CreateUserRequest
//{
//    public string Email { get; set; } = default!;
//    public string DisplayName { get; set; } = default!;
//    public string Password { get; set; } = default!;
//    public UserType UserType { get; set; }
//    public List<string> Roles { get; set; } = new();
//}

//public class CreateUserResponse
//{
//    public Guid Id { get; set; }
//    public string Email { get; set; } = default!;
//    public string DisplayName { get; set; } = default!;
//    public UserType UserType { get; set; }
//    public bool IsActive { get; set; }
//    public List<string> Roles { get; set; } = new();
//}

//public class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
//{
//    private readonly IMediator _mediator;

//    public CreateUserEndpoint(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    public override void Configure()
//    {
//        Post("/users");
//        Roles(RoleNames.SystemAdmin);
//        Summary(s =>
//        {
//            s.Summary = "Create a new IAM user (SystemAdmin only)";
//        });
//    }

//    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
//    {
//        var command = new CreateUserCommand(
//            req.Email,
//            req.DisplayName,
//            req.Password,
//            req.UserType,
//            req.Roles
//        );

//        UserDto user = await _mediator.Send(command, ct);

//        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
//        HttpContext.Response.Headers.Location = $"/users/{user.Id}";

//        await HttpContext.Response.WriteAsJsonAsync(new CreateUserResponse
//        {
//            Id = user.Id,
//            Email = user.Email,
//            DisplayName = user.DisplayName,
//            UserType = user.UserType,
//            IsActive = user.IsActive,
//            Roles = user.Roles.ToList()
//        }, cancellationToken: ct);
//    }
//}
