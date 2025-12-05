namespace BankingSuite.IamService.Infrastructure.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int AccessTokenLifetimeMinutes { get; set; } = 60;
}