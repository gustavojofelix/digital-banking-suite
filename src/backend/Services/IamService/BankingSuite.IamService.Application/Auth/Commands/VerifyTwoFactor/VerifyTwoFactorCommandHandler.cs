using System;
using System.Collections.Generic;
using System.Text;
using BankingSuite.IamService.Application.Auth.Commands.Login;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<VerifyTwoFactorCommand, LoginResult>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly IJwtTokenGenerator _jwtTokenGenerator =
        jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));

    private string TwoFactorProvider = TokenOptions.DefaultEmailProvider;

    public async Task<LoginResult> Handle(
        VerifyTwoFactorCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null || !user.IsActive)
            throw new InvalidOperationException("Invalid 2FA request.");

        if (!user.TwoFactorEnabled)
            throw new InvalidOperationException(
                "Two-factor authentication is not enabled for this user."
            );

        if (!user.EmailConfirmed)
            throw new InvalidOperationException("Email must be confirmed.");

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            TwoFactorProvider,
            request.Code
        );

        if (!isValid)
            throw new InvalidOperationException("Invalid or expired 2FA code.");
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        // For now, keep in sync with JwtOptions.AccessTokenMinutes (60 minutes by default)
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        return new LoginResult
        {
            RequiresTwoFactor = false,
            UserId = user.Id,
            AccessToken = token,
            ExpiresAt = expiresAt,
        };
    }
}
