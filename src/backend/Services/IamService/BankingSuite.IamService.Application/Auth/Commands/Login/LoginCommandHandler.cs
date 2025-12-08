using BankingSuite.BuildingBlocks.Domain.Abstractions;
using BankingSuite.IamService.Application.Auth;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator,
    IEmailSender emailSender)
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager
        ?? throw new ArgumentNullException(nameof(userManager));

    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator
        ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));

    private readonly IEmailSender _emailSender = emailSender
        ?? throw new ArgumentNullException(nameof(emailSender));

    // Identity's default email provider name
    private string TwoFactorProvider = TokenOptions.DefaultEmailProvider;

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
            throw new InvalidOperationException("Invalid credentials.");

        if (!user.EmailConfirmed)
            throw new InvalidOperationException("Email address must be confirmed before logging in.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new InvalidOperationException("Invalid credentials.");

        // If 2FA is not enabled, behave as before.
        if (!user.TwoFactorEnabled)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            // For now, keep in sync with JwtOptions.AccessTokenMinutes (60 minutes by default)
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            return new LoginResult
            { 
                RequiresTwoFactor = false,
                UserId = user.Id,
                AccessToken = token,
                ExpiresAt = expiresAt
            };
        }

        // 2FA enabled: generate a one-time code and email it.
        var twoFactorCode = await _userManager.GenerateTwoFactorTokenAsync(user, TwoFactorProvider);

        var subject = "Your Alvor Bank 2FA code";
        var body = $"""
                    <p>Hello {user.FullName ?? user.Email},</p>
                    <p>Your one-time 2FA code is:</p>
                    <p><strong>{twoFactorCode}</strong></p>
                    <p>This code will expire shortly. If you did not attempt to log in, please contact support.</p>
                    """;

        await _emailSender.SendEmailAsync(user.Email!, subject, body, cancellationToken);

        // Return a result that indicates 2FA is still pending.
        return new LoginResult
        {
            RequiresTwoFactor = true,
            UserId = user.Id,
            AccessToken = null,
            ExpiresAt = null
        };
    }
}
