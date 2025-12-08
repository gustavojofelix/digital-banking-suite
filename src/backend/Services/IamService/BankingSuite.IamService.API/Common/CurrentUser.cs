using System.Security.Claims;
using BankingSuite.IamService.Application.Common.Interfaces;

namespace BankingSuite.IamService.API.Common;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null || !user.Identity?.IsAuthenticated == true)
                throw new InvalidOperationException("No authenticated user in context.");

            // Adjust this if your JWT uses a different claim for user id.
            var idValue =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"); // fallback to 'sub' if you use that

            if (string.IsNullOrWhiteSpace(idValue))
                throw new InvalidOperationException("User id claim is missing.");

            return Guid.Parse(idValue);
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}
