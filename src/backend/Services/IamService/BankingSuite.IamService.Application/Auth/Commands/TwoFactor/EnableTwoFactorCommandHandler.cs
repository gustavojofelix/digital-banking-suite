using System;
using System.Collections.Generic;
using System.Text;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.TwoFactor;

public sealed class EnableTwoFactorCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUser currentUser
) : IRequestHandler<EnableTwoFactorCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly ICurrentUser _currentUser =
        currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task<Unit> Handle(
        EnableTwoFactorCommand request,
        CancellationToken cancellationToken
    )
    {
        if (!_currentUser.IsAuthenticated)
            throw new InvalidOperationException("User must be authenticated.");

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());

        if (user is null)
            throw new KeyNotFoundException("Current user not found.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!passwordValid)
            throw new InvalidOperationException("Invalid password.");

        user.TwoFactorEnabled = true;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to enable 2FA: {errors}");
        }

        return Unit.Value;
    }
}
