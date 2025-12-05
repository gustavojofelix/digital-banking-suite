using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Application.Users.Dto;
using MediatR;

namespace BankingSuite.IamService.Application.Users;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        => _identityService.CreateUserAsync(
            request.Email,
            request.DisplayName,
            request.Password,
            request.UserType,
            request.Roles,
            cancellationToken);
}