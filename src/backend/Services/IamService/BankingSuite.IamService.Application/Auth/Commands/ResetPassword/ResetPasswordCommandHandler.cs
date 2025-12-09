using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            // Do not reveal.
            return Unit.Value;
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            request.Token,
            request.NewPassword
        );

        if (!result.Succeeded && HasConcurrencyError(result))
        {
            // Retry once with a fresh user instance to avoid stale concurrency stamps.
            user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Unit.Value;

            result = await _userManager.ResetPasswordAsync(
                user,
                request.Token,
                request.NewPassword
            );
        }

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to reset password: {errors}");
        }

        return Unit.Value;
    }

    private static bool HasConcurrencyError(IdentityResult result) =>
        result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure));
}

