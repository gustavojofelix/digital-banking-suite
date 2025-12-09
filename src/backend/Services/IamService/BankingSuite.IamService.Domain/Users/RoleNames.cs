namespace BankingSuite.IamService.Domain.Users;

public static class RoleNames
{
    public const string SystemAdmin = "SystemAdmin";
    public const string BackOfficeAgent = "BackOfficeAgent";
    public const string KycOfficer = "KycOfficer";
    public const string Teller = "Teller";
    public const string SupportAgent = "SupportAgent";
    public const string Customer = "Customer";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SystemAdmin,
        BackOfficeAgent,
        KycOfficer,
        Teller,
        SupportAgent,
        Customer
    };
}