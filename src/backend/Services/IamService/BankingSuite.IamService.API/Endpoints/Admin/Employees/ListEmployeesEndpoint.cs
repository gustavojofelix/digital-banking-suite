using BankingSuite.BuildingBlocks.Application.Models;
using BankingSuite.IamService.Application.Employees.Dtos;
using BankingSuite.IamService.Application.Employees.Queries;
using BankingSuite.IamService.Application.Employees.Queries.ListEmployees;
using FastEndpoints;
using MediatR;

namespace BankingSuite.IamService.Api.Endpoints.Admin.Employees;

public sealed class ListEmployeesEndpoint(IMediator mediator)
    : Endpoint<ListEmployeesRequest, PagedResult<EmployeeSummaryDto>>
{
    public override void Configure()
    {
        Verbs(Http.GET, Http.POST);
        Routes("/api/iam/admin/employees");
        Roles("IamAdmin", "SuperAdmin"); // adjust role names to your scheme
        // Group<IamAdminGroup>(); // if you use FastEndpoints groups
    }

    public override async Task HandleAsync(ListEmployeesRequest req, CancellationToken ct)
    {
        var pageNumber = req.PageNumber <= 0 ? 1 : req.PageNumber;
        var pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

        var result = await mediator.Send(
            new ListEmployeesQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                Search: req.Search,
                IncludeInactive: req.IncludeInactive),
            ct);

        await Send.OkAsync(result, ct);
    }
}
