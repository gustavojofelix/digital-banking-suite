using BankingSuite.BuildingBlocks.Application.Models;
using MediatR;
using BankingSuite.IamService.Application.Employees.Dtos;

namespace BankingSuite.IamService.Application.Employees.Queries.ListEmployees;

public sealed record ListEmployeesQuery(
    int PageNumber,
    int PageSize,
    string? Search,
    bool IncludeInactive
) : IRequest<PagedResult<EmployeeSummaryDto>>;
