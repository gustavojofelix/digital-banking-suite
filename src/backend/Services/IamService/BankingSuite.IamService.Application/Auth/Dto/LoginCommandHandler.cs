using BankingSuite.IamService.Application.Auth.Dto;
using BankingSuite.IamService.Application.Common.Interfaces;
using MediatR;

namespace BankingSuite.IamService.Application.Auth;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto?>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<LoginResultDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        => _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
}