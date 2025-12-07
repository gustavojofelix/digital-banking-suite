using BankingSuite.IamService.Application.Employees.Commands;
using BankingSuite.IamService.Application.Employees.Commands.ActivateEmployee;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.Api.Endpoints.Admin.Employees;

public sealed class ActivateEmployeeEndpoint(IMediator mediator) : EndpointWithoutRequest<EmployeeStatusRequest>
{
    public override void Configure()
    {
        Post("/api/iam/admin/employees/{id:guid}/activate");
        Roles("IamAdmin", "SuperAdmin");
        // Group<IamAdminGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Read the id from the route
        var id = Route<Guid>("id");

        await mediator.Send(new ActivateEmployeeCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}

public sealed class DeactivateEmployeeEndpoint(IMediator mediator) : EndpointWithoutRequest<EmployeeStatusRequest>
{
    public override void Configure()
    {
        Post("/api/iam/admin/employees/{id:guid}/deactivate");
        Roles("IamAdmin", "SuperAdmin");
        // Group<IamAdminGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Read the id from the route
        var id = Route<Guid>("id");

        await mediator.Send(new DeactivateEmployeeCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}
