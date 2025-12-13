using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BankingSuite.IamService.Application.Common.Interfaces; // wherever IEmailSender lives

namespace BankingSuite.IamService.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        IOptions<EmailSettings> settings,
        ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }


    public async Task SendEmailAsync(string toEmail, string subject, string textBody, CancellationToken cancellationToken = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = string.IsNullOrWhiteSpace(textBody) ? textBody : textBody,
            IsBodyHtml = string.IsNullOrWhiteSpace(textBody) // if we didn’t get a text body, assume HTML
        };

        message.To.Add(new MailAddress(toEmail));

        if (!string.IsNullOrWhiteSpace(textBody))
        {
            var plainView = AlternateView.CreateAlternateViewFromString(
                textBody,
                null,
                "text/plain");

            var htmlView = AlternateView.CreateAlternateViewFromString(
                textBody,
                null,
                "text/html");

            message.AlternateViews.Add(plainView);
            message.AlternateViews.Add(htmlView);
        }

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.UseSsl,
            Credentials = new NetworkCredential(_settings.User, _settings.Password)
        };

        _logger.LogInformation(
            "Sending email to {Email} via {Host}:{Port}",
            toEmail,
            _settings.Host,
            _settings.Port);

        // SmtpClient is not truly async, but this keeps signature compatible
        await client.SendMailAsync(message, cancellationToken);
    }
}
