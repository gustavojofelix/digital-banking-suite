namespace BankingSuite.IamService.Application.Employees.Dtos;

public class EmployeeSummaryDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string? FullName { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool IsActive { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public string[] Roles { get; init; } = [];
}

public sealed class EmployeeDetailsDto : EmployeeSummaryDto
{
    public string? PhoneNumber { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public DateTimeOffset? LastLoginAt { get; init; }
}
