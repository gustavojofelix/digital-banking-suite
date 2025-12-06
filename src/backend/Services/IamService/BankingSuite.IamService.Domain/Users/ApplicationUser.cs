using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Domain.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public UserType UserType { get; set; } = UserType.Employee;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? DeactivatedAtUtc { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
