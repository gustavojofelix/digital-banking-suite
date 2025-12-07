using MediatR;

namespace BankingSuite.IamService.Application.Employees.Commands.ActivateEmployee;

public sealed record ActivateEmployeeCommand(Guid EmployeeId) : IRequest<Unit>;

public sealed record DeactivateEmployeeCommand(Guid EmployeeId) : IRequest<Unit>;
