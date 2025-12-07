using BankingSuite.IamService.Application.Employees.Dtos;
using BankingSuite.IamService.Application.Employees.Queries;
using BankingSuite.IamService.Application.Employees.Queries.GetEmployeeDetails;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.Api.Endpoints.Admin.Employees;

public sealed class GetEmployeeEndpoint(IMediator mediator)
    : EndpointWithoutRequest<EmployeeDetailsDto>
{
    public override void Configure()
    {
        Get("/api/iam/admin/employee/{id:guid}");
        Roles("IamAdmin", "SuperAdmin");
        // Group<IamAdminGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Read the id from the route
        var id = Route<Guid>("id");

        var result = await mediator.Send(
            new GetEmployeeDetailsQuery(id),
            ct);

        if (result is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}
