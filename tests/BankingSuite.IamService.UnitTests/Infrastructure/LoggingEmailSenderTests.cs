using BankingSuite.IamService.Infrastructure.Email;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BankingSuite.IamService.UnitTests.Infrastructure;

public class LoggingEmailSenderTests
{
    [Fact]
    public async Task SendEmailAsync_Should_Log_Message()
    {
        var logger = new TestLogger<LoggingEmailSender>();
        var sender = new LoggingEmailSender(logger);

        await sender.SendEmailAsync("to@test.com", "Subject", "<p>Body</p>");

        logger.Messages.Should().ContainSingle();
        logger.Messages.Single().State.ToString().Should().Contain("to@test.com");
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Messages { get; } = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(new LogEntry(logLevel, state!));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }

    private sealed record LogEntry(LogLevel Level, object State);
}
