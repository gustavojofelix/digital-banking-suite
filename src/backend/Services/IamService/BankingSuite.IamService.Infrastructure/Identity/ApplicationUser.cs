using BankingSuite.IamService.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = default!;
    public UserType UserType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}