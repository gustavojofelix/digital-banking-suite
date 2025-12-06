using BankingSuite.BuildingBlocks.Application.CQRS;
using BankingSuite.BuildingBlocks.Domain.Abstractions;

namespace BankingSuite.IamService.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password)
    : ICommand<Result<LoginResult>>;

public sealed record LoginResult(string AccessToken, DateTime ExpiresAtUtc);
