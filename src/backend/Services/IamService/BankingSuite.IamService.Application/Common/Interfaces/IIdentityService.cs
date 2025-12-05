using BankingSuite.IamService.Application.Auth.Dto;
using BankingSuite.IamService.Application.Users.Dto;
using BankingSuite.IamService.Domain.Users;

namespace BankingSuite.IamService.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<LoginResultDto?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserDto> CreateUserAsync(
        string email,
        string displayName,
        string password,
        UserType userType,
        IReadOnlyList<string> roles,
        CancellationToken cancellationToken = default);
}