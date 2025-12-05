using BankingSuite.IamService.Application.Users.Dto;
using BankingSuite.IamService.Domain.Users;
using MediatR;

namespace BankingSuite.IamService.Application.Users;

public sealed record CreateUserCommand(
    string Email,
    string DisplayName,
    string Password,
    UserType UserType,
    IReadOnlyList<string> Roles
) : IRequest<UserDto>;