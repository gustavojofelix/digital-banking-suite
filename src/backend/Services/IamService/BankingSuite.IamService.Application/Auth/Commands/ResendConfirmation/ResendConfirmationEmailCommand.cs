using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.ResendConfirmation;

public sealed record ResendConfirmationEmailCommand(string Email, string ConfirmationBaseUrl)
    : IRequest<Unit>;
