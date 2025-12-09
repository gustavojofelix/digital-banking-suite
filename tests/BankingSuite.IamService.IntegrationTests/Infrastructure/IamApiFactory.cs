using System.Linq;
using BankingSuite.IamService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankingSuite.IamService.IntegrationTests.Infrastructure;

public sealed class IamApiFactory : WebApplicationFactory<Program>
{
    public List<TestEmailMessage> SentEmails { get; } = [];

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices(services =>
        {
            // Override IEmailSender with test double
            var emailDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IEmailSender)
            );

            if (emailDescriptor is not null)
            {
                services.Remove(emailDescriptor);
            }

            services.AddSingleton<IEmailSender>(_ => new TestEmailSender(SentEmails));
        });
    }
}

public sealed record TestEmailMessage(string To, string Subject, string HtmlBody);

public sealed class TestEmailSender(List<TestEmailMessage> store) : IEmailSender
{
    private readonly List<TestEmailMessage> _store = store;

    public Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        _store.Add(new TestEmailMessage(toEmail, subject, htmlBody));
        return Task.CompletedTask;
    }
}
