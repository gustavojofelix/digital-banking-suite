using BankingSuite.IamService.Application.Employees.Dtos;
using BankingSuite.IamService.Application.Employees.Queries;
using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.Application.Employees.Queries.GetEmployeeDetails;

public sealed class GetEmployeeDetailsQueryHandler
    : IRequestHandler<GetEmployeeDetailsQuery, EmployeeDetailsDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetEmployeeDetailsQueryHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<EmployeeDetailsDto?> Handle(
        GetEmployeeDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new EmployeeDetailsDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            TwoFactorEnabled = user.TwoFactorEnabled,
            PhoneNumber = user.PhoneNumber,
            LockoutEnd = user.LockoutEnd,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToArray()
        };
    }
}
