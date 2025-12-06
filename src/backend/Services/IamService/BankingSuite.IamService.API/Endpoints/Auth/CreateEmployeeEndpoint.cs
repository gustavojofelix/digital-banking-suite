using BankingSuite.IamService.Application.Auth.Commands.CreateEmployee;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.API.Endpoints.Auth;

public class CreateEmployeeRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

public class CreateEmployeeResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
}

public class CreateEmployeeEndpoint : Endpoint<CreateEmployeeRequest, CreateEmployeeResponse>
{
    private readonly IMediator _mediator;

    public CreateEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/auth/employees");
        AllowAnonymous(); // 👈 TEMPORARY for dev/bootstrap
        // In Chapter 08 we will restrict this to Admins:
        // Roles("Admin");

        Summary(s =>
        {
            s.Summary = "Create a new employee user.";
            s.Description =
                "Creates a bank employee login. In production this would be restricted to Admins.";
        });
    }

    public override async Task HandleAsync(CreateEmployeeRequest req, CancellationToken ct)
    {
        var command = new CreateEmployeeCommand(
            req.Email,
            req.Password,
            req.FirstName,
            req.LastName
        );

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            AddError(result.Error ?? "Could not create employee.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var value = result.Value!;

        await Send.OkAsync(
            new CreateEmployeeResponse
            {
                Id = value.Id,
                Email = value.Email,
                FullName = value.FullName,
            },
            cancellation: ct
        );
    }
}
