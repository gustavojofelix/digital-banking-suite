using BankingSuite.BuildingBlocks.Application.CQRS;
using BankingSuite.BuildingBlocks.Domain.Abstractions;

namespace BankingSuite.IamService.Application.Auth.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName)
    : ICommand<Result<CreateEmployeeResult>>;

public sealed record CreateEmployeeResult(
    Guid Id,
    string Email,
    string FullName);
