using BankingSuite.IamService.Domain.Users;

namespace BankingSuite.IamService.Application.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
