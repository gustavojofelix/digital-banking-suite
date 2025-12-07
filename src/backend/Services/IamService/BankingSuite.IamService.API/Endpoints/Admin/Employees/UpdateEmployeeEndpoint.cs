using BankingSuite.IamService.Application.Employees.Commands;
using BankingSuite.IamService.Application.Employees.Commands.UpdateEmployee;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.Api.Endpoints.Admin.Employees;

public sealed class UpdateEmployeeEndpoint(IMediator mediator)
    : Endpoint<UpdateEmployeeRequest>
{
    public override void Configure()
    {
        Put("/api/iam/admin/employees/{id:guid}");
        Roles("IamAdmin", "SuperAdmin");
        // Group<IamAdminGroup>();
    }

    public override async Task HandleAsync(UpdateEmployeeRequest req, CancellationToken ct)
    {
        var cmd = new UpdateEmployeeCommand(
            EmployeeId: req.Id,
            FullName: req.FullName,
            PhoneNumber: req.PhoneNumber,
            Roles: req.Roles);

        await mediator.Send(cmd, ct);

        await Send.NoContentAsync(ct);
    }
}
