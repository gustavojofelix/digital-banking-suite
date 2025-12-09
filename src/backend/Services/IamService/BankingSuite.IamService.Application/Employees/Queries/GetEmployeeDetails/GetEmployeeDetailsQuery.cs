using BankingSuite.IamService.Application.Employees.Dtos;
using MediatR;

namespace BankingSuite.IamService.Application.Employees.Queries.GetEmployeeDetails;

public sealed record GetEmployeeDetailsQuery(Guid EmployeeId)
    : IRequest<EmployeeDetailsDto?>;
