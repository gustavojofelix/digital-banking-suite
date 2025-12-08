using System.Web;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationEmailCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender
) : IRequestHandler<ResendConfirmationEmailCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager =
        userManager ?? throw new ArgumentNullException(nameof(userManager));

    private readonly IEmailSender _emailSender =
        emailSender ?? throw new ArgumentNullException(nameof(emailSender));

    public async Task<Unit> Handle(
        ResendConfirmationEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // Security: do NOT reveal if user exists or not.
        if (user is null)
            return Unit.Value;

        if (user.EmailConfirmed)
            return Unit.Value;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);

        var confirmationLink =
            $"{request.ConfirmationBaseUrl}?userId={user.Id}&token={encodedToken}";

        var subject = "Confirm your Alvor Bank employee account";
        var body = $"""
            <p>Hello {user.FullName ?? user.Email},</p>
            <p>Please confirm your employee account by clicking the link below:</p>
            <p><a href="{confirmationLink}">Confirm my account</a></p>
            <p>If you did not request this, you can safely ignore this email.</p>
            """;

        await _emailSender.SendEmailAsync(user.Email!, subject, body, cancellationToken);

        return Unit.Value;
    }
}
