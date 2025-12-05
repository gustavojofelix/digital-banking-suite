using BankingSuite.IamService.Application.Auth.Dto;
using MediatR;

namespace BankingSuite.IamService.Application.Auth;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResultDto?>;