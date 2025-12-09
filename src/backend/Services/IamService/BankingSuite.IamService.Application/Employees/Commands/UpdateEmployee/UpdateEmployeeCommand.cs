using MediatR;

namespace BankingSuite.IamService.Application.Employees.Commands.UpdateEmployee;

public sealed record UpdateEmployeeCommand(
    Guid EmployeeId,
    string? FullName,
    string? PhoneNumber,
    string[] Roles
) : IRequest<Unit>;
