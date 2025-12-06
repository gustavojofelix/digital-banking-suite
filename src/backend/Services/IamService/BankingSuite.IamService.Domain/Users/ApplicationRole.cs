using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Domain.Users;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
