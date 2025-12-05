using BankingSuite.IamService.Domain.Users;

namespace BankingSuite.IamService.Application.Users.Dto;

public sealed record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    UserType UserType,
    bool IsActive,
    IReadOnlyList<string> Roles
);
