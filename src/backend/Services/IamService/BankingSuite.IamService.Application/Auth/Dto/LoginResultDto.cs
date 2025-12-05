namespace BankingSuite.IamService.Application.Auth.Dto;

public sealed record LoginResultDto(
    string AccessToken,
    DateTime ExpiresAt
);