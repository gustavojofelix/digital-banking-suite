using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded && HasConcurrencyError(result))
        {
            // Reload a fresh user instance and retry once to avoid stale concurrency tokens.
            user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            result = await _userManager.ConfirmEmailAsync(user, request.Token);
        }

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to confirm email: {errors}");
        }

        return Unit.Value;
    }

    private static bool HasConcurrencyError(IdentityResult result) =>
        result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure));
}

