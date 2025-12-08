using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender
) : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly IEmailSender _emailSender =
        emailSender ?? throw new ArgumentNullException(nameof(emailSender));

    public async Task<Unit> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
        {
            // Do not reveal that user doesn't exist or isn't confirmed.
            return Unit.Value;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);

        var resetLink =
            $"{request.ResetBaseUrl}?email={HttpUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var subject = "Reset your Alvor Bank employee password";
        var body = $"""
            <p>Hello {user.FullName ?? user.Email},</p>
            <p>You requested to reset your employee password.</p>
            <p>Click the link below to choose a new password:</p>
            <p><a href="{resetLink}">Reset my password</a></p>
            <p>If you did not request this, you can safely ignore this email.</p>
            """;

        await _emailSender.SendEmailAsync(user.Email!, subject, body, cancellationToken);

        return Unit.Value;
    }
}
