using System;
using System.Collections.Generic;
using System.Text;
using BankingSuite.IamService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BankingSuite.IamService.Infrastructure.Email;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger = logger;

    public Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        // In dev, just log the email so we can inspect links/codes in the console
        _logger.LogInformation(
            "DEV EmailSender: To={To}, Subject={Subject}, BodyLength={Length}",
            toEmail,
            subject,
            htmlBody.Length
        );

        return Task.CompletedTask;
    }
}
