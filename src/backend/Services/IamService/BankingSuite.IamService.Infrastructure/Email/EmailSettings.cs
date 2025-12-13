namespace BankingSuite.IamService.Infrastructure.Email;

public sealed class EmailSettings
{
    public string FromName { get; init; } = "Alvor Bank IAM";
    public string FromAddress { get; init; } = "no-reply@alvorbank.test";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public bool UseSsl { get; init; } = true;
}
